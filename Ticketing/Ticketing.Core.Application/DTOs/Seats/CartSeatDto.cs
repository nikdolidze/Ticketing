using Ticketing.Core.Domain.Enums;

namespace Ticketing.Core.Application.DTOs.Seats;

public class CartSeatDto
{
    public int SectionId { get; set; }

    public int RowId { get; set; }

    public int SeatId { get; set; }

    public int SeatNumber { get; set; }

    public SeatStatus Status { get; set; }

    public PriceOptionDto PriceOption { get; set; }
}