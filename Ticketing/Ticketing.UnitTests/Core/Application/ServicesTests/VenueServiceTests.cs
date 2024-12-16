using Moq;
using Ticketing.Core.Application.Interfaces;
using Ticketing.Core.Application.Services;
using Ticketing.Core.Domain.Entities;
using Xunit;

namespace Ticketing.UnitTests.Core.Application.ServicesTests;

public class VenueServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly IVenueService _venueService;

    public VenueServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _venueService = new VenueService(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task GetAll_Returns_List_Of_VenueDto()
    {
        // Arrange
        var venues = new List<Venue>
        {
            new() { Name = "Venue 1", Location = "Location 1" },
            new() { Name = "Venue 2", Location = "Location 2" }
        };

        _mockUnitOfWork.Setup(uow => uow.VenueRepository.ReadAsync())
            .ReturnsAsync(venues);

        // Act
        var result = await _venueService.GetAll();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Venue 1", result[0].Name);
        Assert.Equal("Location 1", result[0].Location);
    }

    [Fact]
    public async Task GetById_Returns_VenueDto_When_Venue_Exists()
    {
        // Arrange
        var venueId = 1;
        var venue = new Venue { Name = "Venue 1", Location = "Location 1" };

        _mockUnitOfWork.Setup(uow => uow.VenueRepository.ReadAsync(venueId))
            .ReturnsAsync(venue);

        // Act
        var result = await _venueService.GetById(venueId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Venue 1", result.Name);
        Assert.Equal("Location 1", result.Location);
    }

    [Fact]
    public async Task GetById_Returns_Null_When_Venue_Does_Not_Exist()
    {
        // Arrange
        var venueId = 1;

        _mockUnitOfWork.Setup(uow => uow.VenueRepository.ReadAsync(venueId))
            .ReturnsAsync((Venue)null);

        // Act
        var result = await _venueService.GetById(venueId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetSections_Returns_List_Of_SectionDto()
    {
        // Arrange
        var venueId = 1;
        var sections = new List<Section>
        {
            new() { Name = "Section 1" },
            new() { Name = "Section 2" }
        };
        var venue = new Venue { Sections = sections };

        _mockUnitOfWork.Setup(uow => uow.VenueRepository.ReadAsync(venueId, x => x.Sections))
            .ReturnsAsync(venue);

        // Act
        var result = await _venueService.GetSections(venueId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Section 1", result[0].Name);
        Assert.Equal("Section 2", result[1].Name);
    }

    [Fact]
    public async Task GetSections_Returns_Empty_List_When_Venue_Has_No_Sections()
    {
        // Arrange
        var venueId = 1;
        var venue = new Venue { Sections = new List<Section>() };

        _mockUnitOfWork.Setup(uow => uow.VenueRepository.ReadAsync(venueId, x => x.Sections))
            .ReturnsAsync(venue);

        // Act
        var result = await _venueService.GetSections(venueId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}