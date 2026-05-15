using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using WebhookPayments.Domain.Enums;

namespace WebhookPayments.Domain.Entities;

public class WebhookEvent
{
	public WebhookEvent(string externaEventlId,string externalPaymentId, string gatewayName, string rawPayload)
	{
		Id = Guid.NewGuid();
		ExternalEventId = externaEventlId;
		ExternalPaymentId = externalPaymentId;
		GatewayName = gatewayName;
		RawPayload = rawPayload;
		Status = WebhookStatus.Pending;
		ReceivedAt = DateTime.UtcNow;
	}

	public Guid Id { get; private set; }
	public string ExternalEventId { get; private set; }
	public string ExternalPaymentId { get; private set; }
	public string GatewayName { get; private set; }
	public string RawPayload { get; private set; }

	public WebhookStatus Status { get; private set; }
	public DateTime ReceivedAt { get; private set; }
	public DateTime? ProcessedAt { get; private set; }
	public string? ErrorMessage { get; private set; }


	public void MarkAsProcessed () 
	{
		Status = WebhookStatus.Processed;
		ProcessedAt = DateTime.UtcNow;
		ErrorMessage = null;
	}

	public void MarkAsFailed (string errorMessage)
	{
		Status = WebhookStatus.Failed;
		ErrorMessage = errorMessage;
	}

}
