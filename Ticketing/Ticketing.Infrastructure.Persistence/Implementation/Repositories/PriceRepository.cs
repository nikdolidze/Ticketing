using Ticketing.Core.Application.Interfaces.Repositories;
using Ticketing.Core.Domain.Entities;

namespace Ticketing.Infrastructure.Persistence.Implementation.Repositories;

public class PriceRepository : Repository<Price, int>, IPriceRepository
{
    public PriceRepository(TicketingDataContext context) : base(context)
    {
    }
}