using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Ticketing.Controllers;
using Ticketing.Core.Application.DTOs.Cart;
using Ticketing.Core.Application.Services;
using Ticketing.Core.Domain.Enums;
using Xunit;

namespace Ticketing.UnitTests.Presentation.ControllerTests;

public class OrdersControllerTests
{
    private readonly Mock<ICartService> _mockCartService;
    private readonly OrdersController _controller;

    public OrdersControllerTests()
    {
        _mockCartService = new Mock<ICartService>();
        _controller = new OrdersController(_mockCartService.Object);
    }

    [Fact]
    public async Task GetCartItems_ShouldReturnOk_WhenCartItemsExist()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var cartItems = new List<CartItemDto>
        {
            new() { },
            new() { }
        };

        _mockCartService.Setup(cs => cs.GetCardItems(cartId)).ReturnsAsync(cartItems);

        // Act
        var result = await _controller.GetCartItems(cartId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult.Value.Should().BeEquivalentTo(cartItems);
    }

    [Fact]
    public async Task GetCartItems_ShouldReturnNotFound_WhenCartItemsDoNotExist()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        _mockCartService.Setup(cs => cs.GetCardItems(cartId)).ReturnsAsync(new List<CartItemDto>());

        // Act
        var result = await _controller.GetCartItems(cartId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task AddToCartAsync_ShouldReturnOk_WhenItemIsAdded()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var addToCartDto = new AddToCartDto { };
        var cartState = new AddToCartResponseDto { State = CartState.Active };

        _mockCartService.Setup(cs => cs.AddItemToCartAsync(cartId, addToCartDto)).ReturnsAsync(cartState);

        // Act
        var result = await _controller.AddToCartAsync(cartId, addToCartDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.Value.Should().BeEquivalentTo(cartState);
    }

    [Fact]
    public async Task AddToCartAsync_ShouldReturnBadRequest_WhenExceptionOccurs()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var addToCartDto = new AddToCartDto
        {
        };

        _mockCartService.Setup(cs => cs.AddItemToCartAsync(cartId, addToCartDto)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.AddToCartAsync(cartId, addToCartDto);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task DeleteSeat_ShouldReturnNoContent_WhenSeatIsDeleted()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var eventId = 1;
        var seatId = 1;

        _mockCartService.Setup(cs => cs.DeleteSeatAsync(cartId, eventId, seatId)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteSeat(cartId, eventId, seatId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteSeat_ShouldReturnNotFound_WhenSeatDoesNotExist()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var eventId = 1;
        var seatId = 1;

        _mockCartService.Setup(cs => cs.DeleteSeatAsync(cartId, eventId, seatId)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteSeat(cartId, eventId, seatId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task BookSeats_ShouldReturnOk_WhenBookingIsSuccessful()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var paymentId = Guid.NewGuid();

        _mockCartService.Setup(cs => cs.BookSeatsAsync(cartId)).ReturnsAsync(paymentId);

        // Act
        var result = await _controller.BookSeats(cartId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.Value.Should().Be(paymentId);
    }

    [Fact]
    public async Task BookSeats_ShouldReturnNotFound_WhenBookingFails()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        _mockCartService.Setup(cs => cs.BookSeatsAsync(cartId)).ReturnsAsync((Guid?)null);

        // Act
        var result = await _controller.BookSeats(cartId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}