using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.Dtos;
using WebhookPayments.Application.Usecases;

namespace PaymentService.API.Controllers
{
	[ApiController]
	[Route("api/v1/payments")]
	public class PaymentController : ControllerBase
	{
		private readonly CreatePixPaymentUseCase _pixPaymentCase;

		public PaymentController(CreatePixPaymentUseCase pixPaymentCase)
		{
			_pixPaymentCase = pixPaymentCase;
		}

		[HttpPost(Name = "pix")]
		public async Task<IActionResult> CreatePix([FromBody] PixPaymentRequest request)
		{
			var response = await _pixPaymentCase.ExecuteAsync(request);
			return Ok(response);
		}
	}
}
