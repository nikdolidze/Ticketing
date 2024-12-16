using Ticketing.Core.Domain.Basics;

namespace Ticketing.Core.Domain.Entities;

public class Row : BaseEntity<int>
{
    public int RowNumber { get; set; }

    public Section Section { get; set; }

    public List<Seat> Seats { get; set; } = new();
}