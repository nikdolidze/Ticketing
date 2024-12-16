using Ticketing.Core.Application.Interfaces.Repositories;
using Ticketing.Core.Domain.Entities;

namespace Ticketing.Infrastructure.Persistence.Implementation.Repositories;

public class VenueRepository : Repository<Venue, int>, IVenueRepository
{
    public VenueRepository(TicketingDataContext context) : base(context)
    {
    }
}