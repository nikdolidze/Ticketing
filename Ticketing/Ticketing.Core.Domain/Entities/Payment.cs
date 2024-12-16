using Ticketing.Core.Domain.Basics;
using Ticketing.Core.Domain.Enums;

namespace Ticketing.Core.Domain.Entities;

public class Payment : BaseEntity<int>
{
    public PaymentStatus Status { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Seat> PurchasedSeats { get; set; }
}