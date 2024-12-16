using Ticketing.Core.Application.DTOs.Seats;

namespace Ticketing.Core.Application.DTOs.Cart;

public class CartItemDto
{
    public Guid CartId { get; set; }

    public CartSeatDto Seat { get; set; }
}