using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Ticketing.Core.Application.Interfaces;
using Ticketing.Core.Domain.Basics;

namespace Ticketing.Infrastructure.Persistence.Implementation;

public abstract class Repository<TEntity, TId> : IRepository<TEntity, TId> where TEntity : BaseEntity<TId>
{
    protected readonly TicketingDataContext _context;

    protected Repository(TicketingDataContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Create entity.
    /// </summary>
    /// <param name="entity">Entity.</param>
    public virtual Task CreateAsync(TEntity entity)
    {
        _context.Set<TEntity>().Add(entity);
        return Task.CompletedTask;
    }

    /// <summary>
    ///Return entity by id.
    /// </summary>
    /// <param name="id">Id.</param>
    /// <param name="includeProperties">Include properties</param>
    public virtual async Task<TEntity?> ReadAsync(TId id, params Expression<Func<TEntity, object>>[]? includeProperties)
    {
        IQueryable<TEntity> query = _context.Set<TEntity>();

        if (includeProperties != null && includeProperties.Any())
            query = includeProperties.Aggregate(query,
                (current, includeProperty) => current.Include(includeProperty));

        return await query.FirstOrDefaultAsync(e => e.Id!.Equals(id));
    }

    /// <summary>
    ///Return entities by predicate.
    /// </summary>
    /// <param name="predicate">Predicate.</param>
    public virtual Task<IQueryable<TEntity>> ReadAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return Task.FromResult(_context.Set<TEntity>().Where(predicate));
    }

    /// <summary>
    ///Read All entity.
    /// </summary>
    /// <returns></returns>
    public virtual async Task<IEnumerable<TEntity>> ReadAsync()
    {
        return await _context.Set<TEntity>().ToListAsync();
    }

    /// <summary>
    ///Update entity.
    /// </summary>
    /// <param name="entity">Entity.</param>
    public virtual Task UpdateAsync(TEntity entity)
    {
        _context.Set<TEntity>().Update(entity);
        return Task.CompletedTask;
    }

    /// <summary>
    ///Update entity by id.
    /// </summary>
    /// <param name="id">Id.</param>
    /// <param name="entity">Entity.</param>
    /// <returns></returns>
    public virtual Task UpdateAsync(int id, TEntity entity)
    {
        var existing = _context.Set<TEntity>().Find(id);
        _context.Entry(existing!).CurrentValues.SetValues(entity);
        return Task.CompletedTask;
    }

    /// <summary>
    ///Delete entity by id.
    /// </summary>
    /// <param name="id">Id.</param>
    public virtual async Task DeleteAsync(TId id)
    {
        var item = await ReadAsync(id);
        _context.Set<TEntity>().Remove(item!);
    }

    /// <summary>
    ///Delete entity.
    /// </summary>
    /// <param name="entity">Entity</param>
    public virtual Task DeleteAsync(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity);
        return Task.CompletedTask;
    }

    /// <summary>
    ///Check by predicate.
    /// </summary>
    /// <param name="predicate">Predicate.</param>
    /// <returns></returns>
    public virtual async Task<bool> CheckAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _context.Set<TEntity>().AnyAsync(predicate);
    }
}