using Ticketing.Core.Domain.Enums;

namespace Ticketing.Core.Application.DTOs.Cart;

public class AddToCartDto
{
    public int EventId { get; set; }

    public int SeatId { get; set; }

    public int PriceId { get; set; }
}

public class AddToCartResponseDto
{
    public CartState State { get; set; }

    public decimal TotalAmount { get; set; }
}