using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentService.Application.Usecases;
using WebhookPayments.Application.Usecases;

namespace PaymentService.Application;

public static class DependencyInjection
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		services.AddScoped<CreatePixPaymentUseCase>();
		services.AddScoped<ProcessWebhookUseCase>();

		return services;
	}
}
