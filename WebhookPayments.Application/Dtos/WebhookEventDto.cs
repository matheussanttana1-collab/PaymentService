using System.Text.Json.Serialization;

namespace PaymentService.Application.Dtos;

public class WebhookEventDto
{

	[JsonPropertyName("action")]
	public string Action { get; set; }

	[JsonPropertyName("data")]
	public DataDto Data { get; set; }
	[JsonPropertyName("id")]
	public long EventId { get; set; }
}

public class DataDto
{
	[JsonPropertyName("id")]
	public string Id { get; set; }
}