using PaymentService.Application.Dtos;
using WebhookPayments.Application.Interfaces;
using WebhookPayments.Domain.Entities;
using WebhookPayments.Domain.Interfaces;

namespace WebhookPayments.Application.Usecases;

public class CreatePixPaymentUseCase
{
	private readonly IPaymentGatteway _gatteway;
	private readonly IOrderRepository _repository;

	public CreatePixPaymentUseCase(IPaymentGatteway gatteway, IOrderRepository repository)
	{
		_gatteway = gatteway;
		_repository = repository;
	}

	public async Task<PixPaymentResponse> ExecuteAsync(PixPaymentRequest request) 
	{
		var order = new Order(request.TenantId, request.ExternalOrderId, request.Value);

		var qrCode = await _gatteway.GerarPixQrCode(request.Value, request.Cpf, request.Description, 
		request.Email, request.AcessToken);

		await _repository.AddAsync(order);

		return new PixPaymentResponse(order.Id, qrCode);
	}
}
