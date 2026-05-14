using MassTransit.SagaStateMachine;
using PaymentService.Application.Dtos;
using System.Security.Cryptography.X509Certificates;

namespace WebhookPayments.Application.Interfaces;

public interface IPaymentGateway
{
	Task<PixPaymentResponse> GenaratePixQrCodeAsync(decimal value, string cpf, string description, string email, string AcessToken);
	Task<string> CheckPaymentStatusAsync(string Id);
}
