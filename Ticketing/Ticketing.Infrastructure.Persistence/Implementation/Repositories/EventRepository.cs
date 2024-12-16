using Microsoft.EntityFrameworkCore;
using Ticketing.Core.Application.DTOs.Seats;
using Ticketing.Core.Application.Interfaces.Repositories;
using Ticketing.Core.Domain.Entities;

namespace Ticketing.Infrastructure.Persistence.Implementation.Repositories;

public class EventRepository : Repository<Event, int>, IEventRepository
{
    public EventRepository(TicketingDataContext context) : base(context)
    {
    }

    public async Task<List<SeatDto>> GetSeatsByEventAndSectionAsync(int eventId, int sectionId)
    {
        var seats = await (from seat in _context.Seats
            join row in _context.Rows on seat.Row.Id equals row.Id
            join section in _context.Sections on row.Section.Id equals section.Id
            join venue in _context.Venues on section.Venue.Id equals venue.Id
            join eventEntity in _context.Events on venue.Id equals eventEntity.Venue.Id
            where section.Id == sectionId && eventEntity.Id == eventId
            select new SeatDto
            {
                SectionId = section.Id,
                RowId = row.Id,
                SeatId = seat.Id,
                SeatNumber = seat.SeatNumber,
                Status = seat.SeatStatus,
                PriceOptions = seat.Pricees.Select(p => new PriceOptionDto
                {
                    Id = p.Id,
                    Amount = p.Amount
                }).ToList()
            }).ToListAsync();

        return seats;
    }
}