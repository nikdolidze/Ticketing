using System.ComponentModel.DataAnnotations;
using Ticketing.Core.Domain.Basics;
using Ticketing.Core.Domain.Enums;

namespace Ticketing.Core.Domain.Entities;

public class Seat : BaseEntity<int>
{
    public int SeatNumber { get; set; }

    public SeatStatus SeatStatus { get; set; }

    public Row Row { get; set; }

    public List<Price> Pricees { get; set; }

    [Timestamp]
    public byte[] Version { get; set; }
}