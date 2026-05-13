using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebhookPayment.Infra.Data;
using WebhookPayments.Domain.Entities;
using WebhookPayments.Domain.Interfaces;

namespace WebhookPayment.Infra.Repositories;

public class WebhookRepository : IWebhookRepository
{
	private readonly PaymentsContext _context;

	public WebhookRepository(PaymentsContext context)
	{
		_context = context;
	}

	public async Task AddAsync(WebhookEvent webhookEvent)
	{
		await _context.AddAsync(webhookEvent);
		await _context.SaveChangesAsync();
	}

	public async Task<bool> EventExistsAsync(string externalEventId)
	{
		return await _context.WebhookEvents.AnyAsync(wb => wb.Equals(externalEventId));
	}

	public async Task<IEnumerable<WebhookEvent>> GetPendingEventsAsync()
	{
		return await _context.WebhookEvents.ToListAsync();
	}

	public async Task UpdateAsync(WebhookEvent webhookEvent)
	{
		_context.Update(webhookEvent);
		await _context.SaveChangesAsync();
	}
}
