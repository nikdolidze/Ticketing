using Ticketing.Core.Domain.Enums;

namespace Ticketing.Core.Application.DTOs.Seats;

public class SeatDto
{
    public int SectionId { get; set; }

    public int RowId { get; set; }

    public int SeatId { get; set; }

    public int SeatNumber { get; set; }

    public SeatStatus Status { get; set; }

    public List<PriceOptionDto> PriceOptions { get; set; }
}