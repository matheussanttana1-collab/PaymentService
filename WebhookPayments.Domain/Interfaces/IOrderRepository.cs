
using WebhookPayments.Domain.Entities;

namespace WebhookPayments.Domain.Interfaces;

public interface IOrderRepository
{
	Task<Order> GetByIdAsync(Guid id);
	Task<Order> GetByExternalOrderIdAsync(string externalOrderId);
	Task AddAsync(Order order);
	Task UpdateAsync(Order order);
}
