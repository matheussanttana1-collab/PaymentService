using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.Usecases;

namespace PaymentService.API.Controllers
{
	[ApiController]
	[Route("api/v1/webhooks")]
	public class WebhookController : ControllerBase
	{
		private readonly ProcessWebhookUseCase _webhookCase;

		public WebhookController(ProcessWebhookUseCase webhookCase)
		{
			_webhookCase = webhookCase;
		}

		[HttpPost]
		public async Task<IActionResult> ProcessWebhook () 
		{
			using var reader = new StreamReader(Request.Body);
			var payload = await reader.ReadToEndAsync();

			_webhookCase.ExecuteAsync(payload);

			return Ok();
		}

	}
}
