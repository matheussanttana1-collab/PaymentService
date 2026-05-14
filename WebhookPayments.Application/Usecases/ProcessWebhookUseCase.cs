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
		var payloadJson = JsonSerializer.Deserialize<WebhookDTO>(payload);

		if (payloadJson.Action != "payment.updated" && payloadJson.Action != "payment.created")
			return;
		var paymentId = payloadJson.data.Id;

		if (await _webhookRepo.EventExistsAsync(paymentId))
			return;

		var webhook = new WebhookEvent(paymentId, "MercadoPago", payload);
		await _webhookRepo.AddAsync(webhook);

		var realStatus = await _gateway.CheckPaymentStatusAsync(paymentId);

		var order = await _orderRepo.GetByExternalOrderIdAsync(paymentId);
		if (order is null)
			return;
		if (realStatus == "rejected" || realStatus == "calcelled") 
		{
			order.MarkAsCanceld();
			await _eventBus.Publish(new PaymentReprovedEvent
			{
				OrderId = order.Id.ToString(),
				Amout = order.Amount,
				ProcessedAt = DateTime.UtcNow
			});
		}
		else
		{ 
			order.MarkAsPaid();
			await _eventBus.Publish(new PaymentApproved {
			OrderId = order.Id.ToString() ,
			Amout = order.Amount,
			ProcessedAt = DateTime.UtcNow});	
			
		}
		webhook.MarkAsProcessed();

		await _webhookRepo.AddAsync(webhook);
		await _orderRepo.UpdateAsync(order);

	}
}
