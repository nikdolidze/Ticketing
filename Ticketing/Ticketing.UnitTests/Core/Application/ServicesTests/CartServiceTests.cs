using FluentAssertions;
using MassTransit;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.Logging;
using Moq;
using Ticketing.Core.Application.DTOs.Cart;
using Ticketing.Core.Application.DTOs.Seats;
using Ticketing.Core.Application.Interfaces;
using Ticketing.Core.Application.Services;
using Ticketing.Core.Domain.Entities;
using Ticketing.Core.Domain.Enums;
using Xunit;

namespace Ticketing.UnitTests.Core.Application.ServicesTests;

public class CartServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly CartService _cartService;
    private readonly Mock<IEventService> _evetService;
    private readonly Mock<ILogger<CartService>> _logger;
    private readonly Mock<IPublishEndpoint> _publisher;

    public CartServiceTests()
    {
        _evetService = new Mock<IEventService>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _logger = new Mock<ILogger<CartService>>();
        _publisher = new Mock<IPublishEndpoint>();

        _cartService = new CartService(_mockUnitOfWork.Object, _evetService.Object, _logger.Object, _publisher.Object);
    }

    [Fact]
    public async Task GetCardItems_WhenCartExists_ReturnsCartItems()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var cart = new Cart
        {
            Id = cartId,
            Items = new List<CartItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Seat = new Seat { Id = 1, SeatNumber = 1, SeatStatus = SeatStatus.Available, Row = new Row() { Id = 1 } },
                    Section = new Section { Id = 1 },
                    PriceId = 1,
                    Price = new Price { Id = 1, Amount = 100 }
                }
            }
        };

        _mockUnitOfWork.Setup(u => u.CartRepository.ReadAsync(cartId)).ReturnsAsync(cart);

        var expectedResult = new List<CartItemDto>
        {
            new()
            {
                CartId = cart.Items.First().Id,
                Seat = new CartSeatDto()
                {
                    SeatId = 1,
                    SectionId = 1,
                    SeatNumber = 1,
                    Status = SeatStatus.Available,
                    PriceOption = new PriceOptionDto { Id = 1, Amount = 100 },
                    RowId = 1
                }
            }
        };

        // Act
        var result = await _cartService.GetCardItems(cartId);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task GetCardItems_WhenCartDoesNotExist_ReturnsEmptyList()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        _mockUnitOfWork.Setup(u => u.CartRepository.ReadAsync(cartId)).ReturnsAsync((Cart)null);

        // Act
        var result = await _cartService.GetCardItems(cartId);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCardItems_WhenCartHasNoItems_ReturnsEmptyList()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var cart = new Cart { Id = cartId, Items = new List<CartItem>() };
        _mockUnitOfWork.Setup(u => u.CartRepository.ReadAsync(cartId)).ReturnsAsync(cart);

        // Act
        var result = await _cartService.GetCardItems(cartId);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task AddItemToCartAsync_WhenCartExists_AddsItemToCart()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var addToCartDto = new AddToCartDto { SeatId = 1, PriceId = 1, EventId = 1 };

        var cart = new Cart { Id = cartId, Items = new List<CartItem>(), State = CartState.Active };
        var seat = new Seat { Id = 1, SeatStatus = SeatStatus.Available };
        var price = new Price { Id = 1, Amount = 100 };

        _mockUnitOfWork.Setup(u => u.CartRepository.ReadAsync(cartId)).ReturnsAsync(cart);
        _mockUnitOfWork.Setup(u => u.SeatRepository.ReadAsync(1)).ReturnsAsync(seat);
        _mockUnitOfWork.Setup(u => u.PriceRepository.ReadAsync(1)).ReturnsAsync(price);

        var expectedResponse = new AddToCartResponseDto { State = CartState.Active, TotalAmount = 100 };

        // Act
        var result = await _cartService.AddItemToCartAsync(cartId, addToCartDto);

        // Assert
        _mockUnitOfWork.Verify(u => u.CartRepository.UpdateAsync(cart), Times.Once);
        result.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task AddItemToCartAsync_WhenCartDoesNotExist_CreatesCartAndAddsItem()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var addToCartDto = new AddToCartDto { SeatId = 1, PriceId = 1, EventId = 1 };
        var seat = new Seat { Id = 1, SeatStatus = SeatStatus.Available };
        var price = new Price { Id = 1, Amount = 100 };

        _mockUnitOfWork.Setup(u => u.CartRepository.ReadAsync(cartId)).ReturnsAsync((Cart)null);
        _mockUnitOfWork.Setup(u => u.SeatRepository.ReadAsync(1)).ReturnsAsync(seat);
        _mockUnitOfWork.Setup(u => u.PriceRepository.ReadAsync(1)).ReturnsAsync(price);

        var expectedResponse = new AddToCartResponseDto { State = CartState.Active, TotalAmount = 100 };

        // Act
        var result = await _cartService.AddItemToCartAsync(cartId, addToCartDto);

        // Assert
        _mockUnitOfWork.Verify(u => u.CartRepository.UpdateAsync(It.IsAny<Cart>()), Times.Once);
        result.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task AddItemToCartAsync_WhenSeatDoesNotExist_ThrowsException()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var addToCartDto = new AddToCartDto { SeatId = 1, PriceId = 1, EventId = 1 };

        _mockUnitOfWork.Setup(u => u.CartRepository.ReadAsync(cartId)).ReturnsAsync((Cart)null);
        _mockUnitOfWork.Setup(u => u.SeatRepository.ReadAsync(1)).ReturnsAsync((Seat)null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _cartService.AddItemToCartAsync(cartId, addToCartDto));
    }

    [Fact]
    public async Task AddItemToCartAsync_WhenPriceDoesNotExist_ThrowsException()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var addToCartDto = new AddToCartDto { SeatId = 1, PriceId = 1, EventId = 1 };
        var seat = new Seat { Id = 1, SeatStatus = SeatStatus.Available };
        _mockUnitOfWork.Setup(u => u.CartRepository.ReadAsync(cartId)).ReturnsAsync((Cart)null);
        _mockUnitOfWork.Setup(u => u.SeatRepository.ReadAsync(1)).ReturnsAsync(seat);
        _mockUnitOfWork.Setup(u => u.PriceRepository.ReadAsync(1)).ReturnsAsync((Price)null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _cartService.AddItemToCartAsync(cartId, addToCartDto));
    }

    [Fact]
    public async Task DeleteSeatAsync_WhenSeatExists_RemovesSeatAndReturnsTrue()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var cart = new Cart
        {
            Id = cartId,
            Items = new List<CartItem>
            {
                new() { EventId = 1, SeatId = 1, Seat = new Seat() }
            }
        };

        _mockUnitOfWork.Setup(u => u.CartRepository.ReadAsync(cartId)).ReturnsAsync(cart);

        // Act
        var result = await _cartService.DeleteSeatAsync(cartId, 1, 1);

        // Assert
        result.Should().BeTrue();
        _mockUnitOfWork.Verify(u => u.CartRepository.UpdateAsync(cart), Times.Once);
    }

    [Fact]
    public async Task DeleteSeatAsync_WhenSeatDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var cart = new Cart { Id = cartId, Items = new List<CartItem>() };

        _mockUnitOfWork.Setup(u => u.CartRepository.ReadAsync(cartId)).ReturnsAsync(cart);

        // Act
        var result = await _cartService.DeleteSeatAsync(cartId, 1, 1);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteSeatAsync_WhenCartDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        _mockUnitOfWork.Setup(u => u.CartRepository.ReadAsync(cartId)).ReturnsAsync((Cart)null);

        // Act
        var result = await _cartService.DeleteSeatAsync(cartId, 1, 1);

        // Assert
        result.Should().BeFalse();
    }
}