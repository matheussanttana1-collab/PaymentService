using System.ComponentModel.DataAnnotations;

namespace PaymentService.Application.Dtos;

public record PixPaymentRequest([Required]string TenantId, [Required] string ExternalOrderId,
[Required] decimal Value, [Required] string Cpf, [Required] string Description, [Required] string Email, 
[Required] string AcessToken)
{
}