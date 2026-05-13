

using Microsoft.EntityFrameworkCore;
using WebhookPayment.Infra.Data;
using WebhookPayments.Domain.Entities;
using WebhookPayments.Domain.Interfaces;

namespace WebhookPayment.Infra.Repositories;

public class OrderRepository : IOrderRepository
{
	private readonly PaymentsContext _context;

	public OrderRepository(PaymentsContext context)
	{
		_context = context;
	}

	public async Task AddAsync(Order order)
	{
		await _context.AddAsync(order);
		await _context.SaveChangesAsync();
	}

	public async Task<Order?> GetByExternalOrderIdAsync(string externalOrderId)
	{
		return await _context.Orders.FirstOrDefaultAsync(o => o.ExternalOrderId == externalOrderId);
	}

	public async Task<Order?> GetByIdAsync(Guid id)
	{
		return await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
	}

	public async Task UpdateAsync(Order order)
	{
		_context.Update(order);
		await _context.SaveChangesAsync();
	}
}
