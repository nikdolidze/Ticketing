using Ticketing.Core.Application.DTOs.Seats;
using Ticketing.Core.Domain.Entities;

namespace Ticketing.Core.Application.Interfaces.Repositories;

public interface IEventRepository : IRepository<Event, int>
{
    public Task<List<SeatDto>> GetSeatsByEventAndSectionAsync(int eventId, int sectionId);
}