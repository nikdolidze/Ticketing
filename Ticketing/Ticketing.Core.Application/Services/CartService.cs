using Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ticketing.Core.Application.DTOs.Cart;
using Ticketing.Core.Application.DTOs.Seats;
using Ticketing.Core.Application.Exceptions;
using Ticketing.Core.Application.Interfaces;
using Ticketing.Core.Domain.Entities;
using Ticketing.Core.Domain.Enums;

namespace Ticketing.Core.Application.Services;

public class CartService : ICartService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventService _eventService;
    private readonly ILogger<CartService> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public CartService(IUnitOfWork unitOfWork, IEventService eventService, ILogger<CartService> logger, IPublishEndpoint publishEndpoint)
    {
        _unitOfWork = unitOfWork;
        _eventService = eventService;
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<List<CartItemDto>?> GetCardItems(Guid cartId)
    {
        var cart = await _unitOfWork.CartRepository.ReadAsync(cartId);
        if (cart == null) return new List<CartItemDto>();

        return cart.Items.Select(x => new CartItemDto
        {
            CartId = x.Id,

            Seat = new CartSeatDto
            {
                RowId = x.Seat.Row.Id,
                SeatId = x.Seat.Id,
                SectionId = x.Section.Id,
                SeatNumber = x.Seat.SeatNumber,
                Status = x.Seat.SeatStatus,
                PriceOption = new PriceOptionDto
                {
                    Id = x.Price.Id,
                    Amount = x.Price.Amount
                }
            }
        }).ToList();
    }

    public async Task<AddToCartResponseDto> AddItemToCartAsync(Guid cartId, AddToCartDto addToCartDto)
    {
        var cart = await _unitOfWork.CartRepository.ReadAsync(cartId) ?? new Cart
        {
            State = CartState.Active
        };

        var seat = await _unitOfWork.SeatRepository.ReadAsync(addToCartDto.SeatId);
        if (seat == null) throw new Exception("Seat not found");

        var price = await _unitOfWork.PriceRepository.ReadAsync(addToCartDto.PriceId);
        if (price == null) throw new Exception("price not found");

        var cartItem = new CartItem
        {
            SeatId = addToCartDto.SeatId,
            PriceId = addToCartDto.PriceId,
            EventId = addToCartDto.EventId,
            Price = price
        };

        cart.Items.Add(cartItem);

        await _unitOfWork.CartRepository.UpdateAsync(cart);
        await _unitOfWork.SaveChangesAsync();

        return new AddToCartResponseDto
        {
            State = cart.State,
            TotalAmount = cart.Items.Sum(x => x.Price.Amount)
        };
    }

    public async Task<bool> DeleteSeatAsync(Guid cartId, int eventId, int seatId)
    {
        var cart = await _unitOfWork.CartRepository.ReadAsync(cartId);
        if (cart == null) return false;

        var cartItem = cart.Items
            .FirstOrDefault(item => item.EventId == eventId && item.SeatId == seatId);

        if (cartItem == null) return false;
        cartItem.Seat.SeatStatus = SeatStatus.Available;
        _eventService.InvalidateSeatsCache(cartItem.EventId, cartItem.SectionId);

        cart.Items.Remove(cartItem);
        await _unitOfWork.CartRepository.UpdateAsync(cart);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<Guid?> BookSeatsAsync(Guid cartId)
    {
        var cart = await _unitOfWork.CartRepository.ReadAsync(cartId);
        if (cart == null || !cart.Items.Any()) return null;

        var seatIdsWithStatus = cart.Items.Select(x => x.Seat).Select(x => $@"id {x.Id} status {x.SeatStatus}");
        _logger.LogInformation(string.Join("; ", seatIdsWithStatus));

        foreach (var item in cart.Items)
        {
            if (item.Seat.SeatStatus == SeatStatus.Booked || item.Seat.SeatStatus == SeatStatus.Sold) throw new SeatBookingException($@"seat with id {item.Seat.Id} is already booked or sold");

            item.Seat.SeatStatus = SeatStatus.Booked;
            _eventService.InvalidateSeatsCache(item.EventId, item.SectionId);
        }

        await _unitOfWork.CartRepository.UpdateAsync(cart);

        try
        {
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            foreach (var entry in ex.Entries)
                if (entry.Entity is Seat conflictingSeat)
                    _logger.LogWarning($"Concurrency conflict for seat {conflictingSeat.Id}.");

            throw new DbUpdateConcurrencyException("Failed to book seats due to concurrency conflict.");
        }

        var seatIdsWithStatus2 = cart.Items.Select(x => x.Seat).Select(x => $@"id {x.Id}");
        _logger.LogInformation("Updated seat ids: " + string.Join("; ", seatIdsWithStatus2));

        var paymentId = Guid.NewGuid();
        
        var request = new PaymentCreated
        {
            NotificationTrackingId = Guid.NewGuid(),
            Timestamp = DateTime.UtcNow,
            CustomerEmail = "email",
            CustomerName = "name",
            OrderAmount = 100
        };
        await _publishEndpoint.Publish(request);
        
        return paymentId;
    }
}

public interface ICartService
{
    public Task<List<CartItemDto>?> GetCardItems(Guid cartId);

    Task<AddToCartResponseDto> AddItemToCartAsync(Guid cartId, AddToCartDto addToCartDto);

    Task<bool> DeleteSeatAsync(Guid cartId, int eventId, int seatId);

    Task<Guid?> BookSeatsAsync(Guid cartId);
}