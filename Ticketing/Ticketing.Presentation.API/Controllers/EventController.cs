using Microsoft.AspNetCore.Mvc;
using Ticketing.Core.Application.DTOs.Events;
using Ticketing.Core.Application.DTOs.Seats;
using Ticketing.Core.Application.Services;
using Ticketing.Settings;

namespace Ticketing.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventController(IEventService eventService)
    {
        _eventService = eventService;
    }

    [HttpGet("events")]
    [ResponseCache(Duration = CacheSettings.CacheDuration, Location = ResponseCacheLocation.Client)]
    public async Task<ActionResult<IEnumerable<EventDto>>> GetEvents()
    {
        var result = await _eventService.GetAll();
        return Ok(result);
    }

    [HttpGet("{eventId}/sections/{sectionId}/seats")]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
    public async Task<IActionResult> GetSeats(int eventId, int sectionId)
    {
        var seats = await _eventService.GetSeatsByEventAndSectionAsync(eventId, sectionId);
        return Ok(seats);
    }
}