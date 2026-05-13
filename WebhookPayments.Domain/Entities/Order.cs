using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebhookPayments.Domain.Enums;

namespace WebhookPayments.Domain.Entities;

public class Order
{
	public Order(string tenentId,string externalOrderId,decimal amount)
	{
		Id = Guid.NewGuid();
		TenentId = tenentId;
		ExternalOrderId = externalOrderId;
		Status = OrderStatus.Pending;
		Amount = amount;
		CreatedAt = DateTime.UtcNow;
	}

	public Guid Id {  get; private set; }
	public string TenentId{ get; private set; }
	public string ExternalOrderId{ get; private set; }
	public OrderStatus Status { get; private set; }
	public decimal Amount { get; private set; }
	public DateTime CreatedAt {  get; private set; }
	public DateTime? PaidAt { get; private set; }


	public void MarkAsPaid()
	{
		if (Status == OrderStatus.Paid)
			throw new InvalidOperationException("This order has already been paid previously.");

		if (Status == OrderStatus.Cancelled)
			throw new InvalidOperationException("It is no possible to pay for a canceled order");
		Status = OrderStatus.Paid;
		PaidAt = DateTime.UtcNow;
	}
	public void MarkAsCanceld()
	{
		Status = OrderStatus.Cancelled;
	}


}
