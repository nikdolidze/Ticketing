using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Ticketing.Core.Application.Interfaces.Repositories;
using Ticketing.Core.Domain.Entities;

namespace Ticketing.Infrastructure.Persistence.Implementation.Repositories;

public class CartRepository : Repository<Cart, Guid>, ICartRepository
{
    public CartRepository(TicketingDataContext context) : base(context)
    {
    }

    public override async Task<Cart?> ReadAsync(Guid id, params Expression<Func<Cart, object>>[]? includeProperties)
    {
        return await _context.Carts.Include(x => x.Items)
            .ThenInclude(x => x.Seat).ThenInclude(x => x.Row)
            .Include(x => x.Items).ThenInclude(x => x.Price)
            .Include(x => x.Items).ThenInclude(x => x.Event)
            .Include(x => x.Items).ThenInclude(x => x.Section).FirstOrDefaultAsync(x => x.Id == id);
    }
}