using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Application.Events;

public record PaymentApproved
{
	public string OrderId { get; set; }
	public decimal Amout { get; set; }
	public DateTime ProcessedAt { get; set; }
}
