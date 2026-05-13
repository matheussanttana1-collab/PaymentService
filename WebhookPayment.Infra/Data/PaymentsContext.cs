using Microsoft.EntityFrameworkCore;
using WebhookPayments.Domain.Entities;

namespace WebhookPayment.Infra.Data;

public class PaymentsContext : DbContext
{
	public PaymentsContext(DbContextOptions op) : base (op)
	{
		
	}
	public DbSet<Order> Orders { get; set; }
	public DbSet<WebhookEvent> WebhookEvents { get; set; }
}
