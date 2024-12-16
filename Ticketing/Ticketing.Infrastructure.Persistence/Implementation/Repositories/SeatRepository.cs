using Ticketing.Core.Application.Interfaces.Repositories;
using Ticketing.Core.Domain.Entities;

namespace Ticketing.Infrastructure.Persistence.Implementation.Repositories;

public class SeatRepository : Repository<Seat, int>, ISeatRepository
{
    public SeatRepository(TicketingDataContext context) : base(context)
    {
    }
}