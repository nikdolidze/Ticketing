using Ticketing.Core.Application.DTOs.Events;
using Ticketing.Core.Application.DTOs.Seats;
using Ticketing.Core.Application.Interfaces;

namespace Ticketing.Core.Application.Services;

public class EventService : IEventService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheWrapper _cache;
    private readonly string _eventCacheKey = "events_cache_key";

    public EventService(IUnitOfWork unitOfWork, ICacheWrapper cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<List<EventDto>> GetAll()
    {
        if (_cache.TryGetValue(_eventCacheKey, out List<EventDto> cachedEvents)) return cachedEvents;

        var events = await _unitOfWork.EventRepository.ReadAsync();
        var eventDtos = events.Select(x => new EventDto
        {
            Name = x.Name,
            Date = x.Date
        }).ToList();

        _cache.Set(_eventCacheKey, eventDtos, TimeSpan.FromMinutes(10));

        return eventDtos;
    }

    public async Task<List<SeatDto>> GetSeatsByEventAndSectionAsync(int eventId, int sectionId)
    {
        var cacheKey = $"{_eventCacheKey}_{eventId}_{sectionId}";

        if (!_cache.TryGetValue<List<SeatDto>>(cacheKey, out var cachedSeats))
        {
            // Cache miss - get from repository
            cachedSeats = await _unitOfWork.EventRepository.GetSeatsByEventAndSectionAsync(eventId, sectionId);

            _cache.Set(cacheKey, cachedSeats, TimeSpan.FromMinutes(5));
        }

        // Return cached or fresh data
        return cachedSeats;
    }

    public void InvalidateSeatsCache(int eventId, int sectionId)
    {
        var cacheKey = $"EventSeats_{eventId}_{sectionId}";
        _cache.Remove(cacheKey);
    }
}

public interface IEventService
{
    public Task<List<EventDto>> GetAll();

    public Task<List<SeatDto>> GetSeatsByEventAndSectionAsync(int eventId, int sectionId);

    void InvalidateSeatsCache(int eventId, int sectionId);
}