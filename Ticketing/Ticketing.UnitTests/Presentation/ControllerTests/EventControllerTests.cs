using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Ticketing.Controllers;
using Ticketing.Core.Application.DTOs.Events;
using Ticketing.Core.Application.DTOs.Seats;
using Ticketing.Core.Application.Services;
using Xunit;

namespace Ticketing.UnitTests.Presentation.ControllerTests;

public class EventControllerTests
{
    private readonly Mock<IEventService> _mockEventService;
    private readonly EventController _eventController;

    public EventControllerTests()
    {
        _mockEventService = new Mock<IEventService>();
        _eventController = new EventController(_mockEventService.Object);
    }

    [Fact]
    public async Task GetEvents_ShouldReturnOkResult_WithEventList()
    {
        // Arrange
        var eventDtos = new List<EventDto>
        {
            new() { Name = "Event 1" },
            new() { Name = "Event 2" }
        };

        _mockEventService.Setup(s => s.GetAll()).ReturnsAsync(eventDtos);

        // Act
        var result = await _eventController.GetEvents();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<EventDto>>(okResult.Value);
        Assert.Equal(2, returnValue.Count());
        returnValue.Should().BeEquivalentTo(eventDtos);
    }

    [Fact]
    public async Task GetSeats_ShouldReturnOkResult_WithSeats()
    {
        // Arrange
        var eventId = 1;
        var sectionId = 1;
        var seats = new List<SeatDto>
        {
            new() { SeatId = 1, RowId = 1, SeatNumber = 1 },
            new() { SeatId = 1, RowId = 1, SeatNumber = 2 }
        };

        _mockEventService.Setup(s => s.GetSeatsByEventAndSectionAsync(eventId, sectionId)).ReturnsAsync(seats);

        // Act
        var result = await _eventController.GetSeats(eventId, sectionId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<SeatDto>>(okResult.Value);
        Assert.Equal(2, returnValue.Count());
        returnValue.Should().BeEquivalentTo(seats);
    }
}