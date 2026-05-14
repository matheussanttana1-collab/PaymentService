using PaymentService.Application.Dtos;
using WebhookPayments.Application.Interfaces;
using WebhookPayments.Domain.Entities;
using WebhookPayments.Domain.Interfaces;

namespace WebhookPayments.Application.Usecases;

public class CreatePixPaymentUseCase
{
	private readonly IPaymentGateway _gatteway;
	private readonly IOrderRepository _repository;

	public CreatePixPaymentUseCase(IPaymentGateway gatteway, IOrderRepository repository)
	{
		_gatteway = gatteway;
		_repository = repository;
	}

	public async Task<PixPaymentResponse> ExecuteAsync(PixPaymentRequest request) 
	{

		var response = await _gatteway.GenaratePixQrCodeAsync(request.Value, request.Cpf, request.Description, 
		request.Email, request.AcessToken);

		var order = new Order(request.TenantId, response.id, request.Value);
		await _repository.AddAsync(order);

		return new PixPaymentResponse(order.Id.ToString(), response.QrCode);
	}
}
