using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Ticketing.Controllers;
using Ticketing.Core.Application.DTOs.Sections;
using Ticketing.Core.Application.DTOs.Venues;
using Ticketing.Core.Application.Services;
using Xunit;

namespace Ticketing.UnitTests.Presentation.ControllerTests;

public class VenueControllerTests
{
    private readonly Mock<IVenueService> _mockVenueService;
    private readonly VenueController _controller;

    public VenueControllerTests()
    {
        _mockVenueService = new Mock<IVenueService>();
        _controller = new VenueController(_mockVenueService.Object);
    }

    [Fact]
    public async Task GetVenues_ShouldReturnOk_WhenVenuesExist()
    {
        // Arrange
        var venues = new List<VenueDto>
        {
            new() { Name = "Venue 1", Location = "Location 1" },
            new() { Name = "Venue 2", Location = "Location 2" }
        };

        _mockVenueService.Setup(vs => vs.GetAll()).ReturnsAsync(venues);

        // Act
        var result = await _controller.GetVenues();

        // Assert
        var okObjectResult = result.Result as OkObjectResult;
        okObjectResult.Value.Should().BeEquivalentTo(venues);
    }

    [Fact]
    public async Task GetVenues_ShouldReturnEmpty_WhenNoVenuesExist()
    {
        // Arrange
        var venues = new List<VenueDto>();
        _mockVenueService.Setup(vs => vs.GetAll()).ReturnsAsync(venues);

        // Act
        var result = await _controller.GetVenues();

        // Assert
        var okObjectResult = result.Result as OkObjectResult;
        okObjectResult.Value.Should().BeEquivalentTo(venues);
    }

    [Fact]
    public async Task GetSectionsForVenue_ShouldReturnNotFound_WhenVenueDoesNotExist()
    {
        // Arrange
        var venueId = 1;
        _mockVenueService.Setup(vs => vs.GetById(venueId)).ReturnsAsync((VenueDto)null);

        // Act
        var result = await _controller.GetSectionsForVenue(venueId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetSectionsForVenue_ShouldReturnOk_WhenSectionsExist()
    {
        // Arrange
        var venueId = 1;
        var sections = new List<SectionDto>
        {
            new() { Name = "Section 1" },
            new() { Name = "Section 2" }
        };

        _mockVenueService.Setup(vs => vs.GetById(venueId)).ReturnsAsync(new VenueDto { Name = "Venue 1", Location = "Location 1" });
        _mockVenueService.Setup(vs => vs.GetSections(venueId)).ReturnsAsync(sections);

        // Act
        var result = await _controller.GetSectionsForVenue(venueId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okresult = result.Result as OkObjectResult;
        okresult.Value.Should().BeEquivalentTo(sections);
    }
}