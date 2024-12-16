using Ticketing.Core.Domain.Basics;

namespace Ticketing.Core.Domain.Entities;

public class CartItem : BaseEntity<Guid>
{
    public int SeatId { get; set; }

    public int EventId { get; set; }

    public int SectionId { get; set; }

    public int PriceId { get; set; }

    public Seat Seat { get; set; }

    public Price Price { get; set; }

    public Event Event { get; set; }

    public Section Section { get; set; }
}