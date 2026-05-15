using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using WebhookPayment.Infra.Data;
using WebhookPayment.Infra.Gatteways;
using WebhookPayment.Infra.Repositories;
using WebhookPayments.Application.Interfaces;
using WebhookPayments.Domain.Interfaces;

namespace WebhookPayment.Infra;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(
	this IServiceCollection services, IConfiguration configuration)
	{
		var connectionString = configuration.GetConnectionString("DefaultConnection");

		services.AddDbContext<PaymentsContext>(opts => opts.UseNpgsql(connectionString));

		services.AddScoped<IOrderRepository, OrderRepository>();
		services.AddScoped<IWebhookRepository, WebhookRepository>();

		services.AddHttpClient<IPaymentGateway, MercadoPagoGatteway>(client =>
		{
			client.BaseAddress = new Uri(configuration["MercadoPago:BaseUrl"]);
			client.DefaultRequestHeaders.Authorization =
			new AuthenticationHeaderValue("Bearer", configuration["MP_ACCESS_TOKEN"]);
		});

		services.AddMassTransit(busConfigurator =>
		{
			busConfigurator.UsingRabbitMq((context, rabbitConfigurator) =>
			{
				// Pega as credenciais do RabbitMQ lá do appsettings.json
				rabbitConfigurator.Host(configuration["RMQ_HOST"], "/", hostConfigurator =>
				{
					// Se o PaymentService fosse RECEBER mensagens de outra API, 
					// registraríamos os Consumers aqui:
					// busConfigurator.AddConsumer<AlgumConsumer>()
					hostConfigurator.Username(configuration["RMQ_USER"]);
					hostConfigurator.Password(configuration["RMQ_PASSWORD"]);
				});

				rabbitConfigurator.ConfigureEndpoints(context);
			});
		});

		return services;
	}
}
