using MassTransit;
using PaymentService.Application.Dtos;
using PaymentService.Application.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebhookPayments.Application.Interfaces;
using WebhookPayments.Domain.Entities;
using WebhookPayments.Domain.Enums;
using WebhookPayments.Domain.Interfaces;

namespace PaymentService.Application.Usecases;

public class ProcessWebhookUseCase
{
	private readonly IOrderRepository _orderRepo;
	private readonly IWebhookRepository _webhookRepo;
	private readonly IPublishEndpoint _eventBus;
	private readonly IPaymentGateway _gateway;

	public ProcessWebhookUseCase(IWebhookRepository webhookRepo, IPublishEndpoint eventBus, 
	IPaymentGateway gateway, IOrderRepository orderRepo)
	{
		_webhookRepo = webhookRepo;
		_eventBus = eventBus;
		_gateway = gateway;
		_orderRepo = orderRepo;
	}

	public async Task ExecuteAsync(string payload)
	{
		// =========================================================================
		// 1. DESSERIALIZAÇÃO E FILTRO (O Segurança da Porta)
		// =========================================================================
		var payloadJson = JsonSerializer.Deserialize<WebhookEventDto>(payload);

		if (payloadJson == null || payloadJson.Data == null)
			return;

		if (payloadJson.Action != "payment.updated" && payloadJson.Action != "payment.created")
			return;

		// =========================================================================
		// 2. IDEMPOTÊNCIA (O Escudo contra Duplicações)
		// =========================================================================
		var webhookEventId = payloadJson.EventId.ToString();

		if (await _webhookRepo.EventExistsAsync(webhookEventId))
			return;

		// =========================================================================
		// 3. REGISTRO INICIAL DE AUDITORIA (Garante que não perderemos a notificação)
		// =========================================================================
		var paymentId = payloadJson.Data.Id;
		var webhook = new WebhookEvent(webhookEventId, paymentId, "MercadoPago", payload);

		// CORREÇÃO 2: Salva no banco IMEDIATAMENTE como Pendente.
		await _webhookRepo.AddAsync(webhook);

		try
		{
			// =========================================================================
			// 4. SEGURANÇA E PREPARAÇÃO (A Dupla Checagem)
			// =========================================================================
			var realStatus = await _gateway.CheckPaymentStatusAsync(paymentId);
			var order = await _orderRepo.GetByExternalOrderIdAsync(paymentId);

			if (order is null)
			{
				// Se a Order não for nossa, finalizamos o Webhook com sucesso para encerrar o ciclo.
				webhook.MarkAsProcessed();
				await _webhookRepo.UpdateAsync(webhook);
				return;
			}

			// =========================================================================
			// 5. REGRA DE NEGÓCIO (A Orquestração do Pedido)
			// =========================================================================
			if (realStatus == "rejected" || realStatus == "cancelled")
			{
				order.MarkAsCanceld();

				await _eventBus.Publish(new PaymentReprovedEvent
				{
					OrderId = order.Id.ToString(),
					Amout = order.Amount,
					ProcessedAt = DateTime.UtcNow
				});

				await _orderRepo.UpdateAsync(order);
			}
			// CORREÇÃO 1: 'else if' protegendo a aplicação de pagamentos "pending" ou "in_process"
			else if (realStatus == "approved" && order.Status != OrderStatus.Paid)
			{
				order.MarkAsPaid();

				await _eventBus.Publish(new PaymentApproved
				{
					OrderId = order.Id.ToString(),
					Amout = order.Amount,
					ProcessedAt = DateTime.UtcNow
				});

				await _orderRepo.UpdateAsync(order);
			}

			// =========================================================================
			// 6. CONCLUSÃO COM SUCESSO
			// =========================================================================
			// Se chegou até aqui sem dar erro (mesmo sendo um status pending que ignorou os IFs),
			// nós marcamos como processado e atualizamos o banco.
			webhook.MarkAsProcessed();
			await _webhookRepo.UpdateAsync(webhook);
		}
		catch (Exception ex)
		{
			// =========================================================================
			// 7. CONTROLE DE DANOS (O Sistema de Incêndio)
			// =========================================================================
			// Se a internet, o RabbitMQ ou o Banco caírem no meio da operação,
			// nós atualizamos aquele registro inicial com o motivo da falha.
			webhook.MarkAsFailed(ex.Message);
			await _webhookRepo.UpdateAsync(webhook);

			// Relança a exceção para retornar um erro 500 e obrigar o Mercado Pago a tentar novamente depois
			throw;
		}
	}
}
