using Microsoft.AspNetCore.Mvc;
using Ticketing.Core.Application.DTOs.Sections;
using Ticketing.Core.Application.DTOs.Venues;
using Ticketing.Core.Application.Services;

namespace Ticketing.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VenueController : ControllerBase
{
    private readonly IVenueService _venueService;

    public VenueController(IVenueService venueService)
    {
        _venueService = venueService;
    }

    [HttpGet("venues")]
    public async Task<ActionResult<IEnumerable<VenueDto>>> GetVenues()
    {
        var result = await _venueService.GetAll();
        return Ok(result);
    }

    [HttpGet("{venueId}/sections")]
    public async Task<ActionResult<IEnumerable<SectionDto>>> GetSectionsForVenue(int venueId)
    {
        var venue = await _venueService.GetById(venueId);
        if (venue is null) return NoContent();

        var result = await _venueService.GetSections(venueId);
        return Ok(result);
    }
}