using PaymentService.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebhookPayments.Application.Interfaces;

namespace WebhookPayment.Infra.Gatteways;

public class MercadoPagoGatteway : IPaymentGateway
{
	private readonly HttpClient _client;

	public MercadoPagoGatteway(HttpClient client, string acessToken)
	{
		_client = client;
	}

	public async Task<string> CheckPaymentStatusAsync(string Id)
	{
		// 1. Fazemos um GET na rota oficial do Mercado Pago usando o ID fornecido
		// Como o HttpClient já tem o seu Access Token no Header, ninguém consegue forjar essa chamada.
		var response = await _client.GetAsync($"v1/payments/{Id}");

		if (!response.IsSuccessStatusCode)
			return "rejected";

		var responseJson = await response.Content.ReadAsStringAsync();
		var mercadoPagoResult = JsonSerializer.Deserialize<JsonElement>(responseJson);

		return mercadoPagoResult.GetProperty("status").GetString()!;
	}

	public async Task<PixPaymentResponse> GenaratePixQrCodeAsync(decimal amount, string cpf, string description, string email, 
	string AcessToken)
	{
		_client.DefaultRequestHeaders.Authorization =
			new AuthenticationHeaderValue("Bearer", AcessToken);
		// 1. Monta o DTO com o formato EXATO que o Mercado Pago exige
		var requestBody = new
		{
			transaction_amount = amount,
			description = description,
			payment_method_id = "pix",
			payer = new
			{
				email = email,
				cpf = cpf
			}
		};

		// 2. Transforma em JSON e faz o POST pra rota do Mercado Pago
		var content = new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8,
		"application/json");

		var response = await _client.PostAsync("v1/payments", content);

		//Gera uma exception caso nao Seja Sucesso
		response.EnsureSuccessStatusCode();

		// 4. Lê a resposta, extrai o "Copia e Cola" do Pix e devolve pro seu Use Case
		var responseJson = await response.Content.ReadAsStringAsync();
		var mercadoPagoResult = JsonSerializer.Deserialize<JsonElement>(responseJson);

		var externald = mercadoPagoResult.GetProperty("id").ToString();
		var qrCode =  mercadoPagoResult.GetProperty("point_of_interaction").GetProperty("transaction_data")
		.GetProperty("qr_code").GetString()!;

		return new PixPaymentResponse(externald, qrCode);
	}
}
