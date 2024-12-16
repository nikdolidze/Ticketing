using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Ticketing.Core.Application.DTOs.Cart;
using Ticketing.Core.Application.Services;
using Ticketing.Filters;

namespace Ticketing.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ICartService _cartService;

    /// <inheritdoc />
    public OrdersController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet("carts/{cartId}")]
    public async Task<ActionResult<IEnumerable<CartItemDto>>> GetCartItems(Guid cartId)
    {
        var cartItems = await _cartService.GetCardItems(cartId);

        if (cartItems == null || cartItems.IsNullOrEmpty()) return NotFound();

        return Ok(cartItems);
    }

    [HttpPost("{cartId}")]
    public async Task<IActionResult> AddToCartAsync(Guid cartId, [FromBody] AddToCartDto addToCartDto)
    {
        try
        {
            var cartState = await _cartService.AddItemToCartAsync(cartId, addToCartDto);
            return Ok(cartState);
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    [HttpDelete("carts/{cartId}/events/{eventId}/seats/{seatId}")]
    public async Task<IActionResult> DeleteSeat(Guid cartId, int eventId, int seatId)
    {
        var result = await _cartService.DeleteSeatAsync(cartId, eventId, seatId);

        if (result) return NoContent();

        return NotFound();
    }

    [HttpPut("carts/{cartId}/book")]
    [ServiceFilter(typeof(BookSeatsExceptionFilter))]
    public async Task<IActionResult> BookSeats(Guid cartId)
    {
        var paymentId = await _cartService.BookSeatsAsync(cartId);
        if (paymentId == null) return NotFound();
        return Ok(paymentId);
    }
}