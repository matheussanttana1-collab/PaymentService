using System.Text.Json.Serialization;

namespace PaymentService.Application.Dtos;

public class WebhookDTO
{

	[JsonPropertyName("action")]
	public string Action { get; set; }

	[JsonPropertyName("data")]
	public DataDto data { get; set; }
}

public class DataDto
{
	[JsonPropertyName("id")]
	public string Id { get; set; }
}