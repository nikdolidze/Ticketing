using FluentAssertions;
using Moq;
using Ticketing.Core.Application.DTOs.Events;
using Ticketing.Core.Application.DTOs.Seats;
using Ticketing.Core.Application.Interfaces;
using Ticketing.Core.Application.Services;
using Ticketing.Core.Domain.Entities;
using Xunit;

namespace Ticketing.UnitTests.Core.Application.ServicesTests;

public class EventServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly EventService _eventService;
    private readonly Mock<ICacheWrapper> _cacheService;

    public EventServiceTests()
    {
        _cacheService = new();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _eventService = new EventService(_mockUnitOfWork.Object, _cacheService.Object);
    }

    [Fact]
    public async Task GetAll_WhenEventsExist_ReturnsListOfEventDtos()
    {
        // Arrange
        var events = new List<Event>
        {
            new() { Name = "Concert", Date = DateTime.Now },
            new() { Name = "Festival", Date = DateTime.Now.AddDays(1) }
        };

        _mockUnitOfWork.Setup(u => u.EventRepository.ReadAsync()).ReturnsAsync(events);

        var expectedResult = events.Select(x => new EventDto
        {
            Name = x.Name,
            Date = x.Date
        }).ToList();

        // Act
        var result = await _eventService.GetAll();

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task GetAll_WhenNoEventsExist_ReturnsEmptyList()
    {
        // Arrange
        _mockUnitOfWork.Setup(u => u.EventRepository.ReadAsync()).ReturnsAsync(new List<Event>());

        // Act
        var result = await _eventService.GetAll();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSeatsByEventAndSectionAsync_WhenSeatsExist_ReturnsListOfSeatDtos()
    {
        // Arrange
        var eventId = 1;
        var sectionId = 1;

        var seatDtos = new List<SeatDto>
        {
            new() { SeatId = 1, SeatNumber = 101, RowId = 1 },
            new() { SeatId = 2, SeatNumber = 102, RowId = 1 }
        };

        _mockUnitOfWork.Setup(u => u.EventRepository.GetSeatsByEventAndSectionAsync(eventId, sectionId))
            .ReturnsAsync(seatDtos);

        // Act
        var result = await _eventService.GetSeatsByEventAndSectionAsync(eventId, sectionId);

        // Assert
        result.Should().BeEquivalentTo(seatDtos);
    }

    [Fact]
    public async Task GetSeatsByEventAndSectionAsync_WhenNoSeatsExist_ReturnsEmptyList()
    {
        // Arrange
        var eventId = 1;
        var sectionId = 1;
        string _eventCacheKey = "events_cache_key";
        var cacheKey = $"{_eventCacheKey}_{eventId}_{sectionId}";

        _mockUnitOfWork.Setup(u => u.EventRepository.GetSeatsByEventAndSectionAsync(eventId, sectionId))
            .ReturnsAsync(new List<SeatDto>());

        // Act
        var result = await _eventService.GetSeatsByEventAndSectionAsync(eventId, sectionId);

        // Assert
        _cacheService.Verify(c => c.TryGetValue<List<SeatDto>>(cacheKey, out It.Ref<List<SeatDto>>.IsAny), Times.Once);
        result.Should().BeEmpty();
    }
}