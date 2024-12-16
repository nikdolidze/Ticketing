using System.Linq.Expressions;
using Ticketing.Core.Domain.Basics;

namespace Ticketing.Core.Application.Interfaces;

public interface IRepository<TEntity, Tid> where TEntity : BaseEntity<Tid>
{
    Task CreateAsync(TEntity entity);

    Task<TEntity?> ReadAsync(Tid id, params Expression<Func<TEntity, object>>[]? includeProperties);

    Task<IEnumerable<TEntity>> ReadAsync();

    Task<IQueryable<TEntity>> ReadAsync(Expression<Func<TEntity, bool>> predicate);

    Task UpdateAsync(TEntity entity);

    Task UpdateAsync(int id, TEntity entity);

    Task DeleteAsync(Tid id);

    Task DeleteAsync(TEntity entity);

    Task<bool> CheckAsync(Expression<Func<TEntity, bool>> predicate);
}