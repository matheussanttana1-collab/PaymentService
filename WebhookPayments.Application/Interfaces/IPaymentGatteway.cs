namespace WebhookPayments.Application.Interfaces;

public interface IPaymentGatteway
{
	Task<string> GerarPixQrCode(decimal value, string cpf, string description, string email, string AcessToken);
}
