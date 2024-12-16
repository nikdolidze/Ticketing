using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Ticketing.Core.Application.DTOs.Cart;
using Ticketing.Core.Application.Services;
using Ticketing.Infrastructure.Persistence;

namespace Ticketing.IntegrationTests.Core.Application.Services.CartService;

[TestFixture]
public class CartServiceTests
{
    private ICartService _cartService;
    private TicketingDataContext _context;

    [SetUp]
    public void Setup()
    {
        var factory = new CustomWebApplicationFactory<Program>();
        var scope = factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;

        _context = scopedServices.GetRequiredService<TicketingDataContext>();
        _cartService = scopedServices.GetRequiredService<ICartService>();
    }

    [Test]
    public async Task AddItemToCartAsync_ShouldAddItem_WhenValidDataProvided()
    {
        // Arrange
        var addToCartDto = new AddToCartDto
        {
            SeatId = 1,
            PriceId = 1,
            EventId = 1
        };

        // Act
        var result = await _cartService.AddItemToCartAsync(Constants.CartId, addToCartDto);

        // Assert
        result.Should().NotBeNull();
        result.TotalAmount.Should().Be(100);
        var updatedCart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == Constants.CartId);
        updatedCart.Should().NotBeNull();
        updatedCart.Items.Should().HaveCount(1);
    }
}