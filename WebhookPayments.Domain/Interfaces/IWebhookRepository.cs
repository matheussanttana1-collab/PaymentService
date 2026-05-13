using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebhookPayments.Domain.Entities;

namespace WebhookPayments.Domain.Interfaces;

public interface IWebhookRepository
{
	Task AddAsync(WebhookEvent webhookEvent);
	Task<bool> EventExistsAsync(string externalEventId); // Para checar idempotência
	Task<IEnumerable<WebhookEvent>> GetPendingEventsAsync(); // Para o nosso Worker puxar
	Task UpdateAsync(WebhookEvent webhookEvent);
}
