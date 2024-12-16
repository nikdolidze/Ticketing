using Ticketing.Core.Domain.Enums;

namespace Ticketing.Core.Application.DTOs.Payment;

public class PaymentDto
{
    public PaymentStatus? Status { get; set; }
}