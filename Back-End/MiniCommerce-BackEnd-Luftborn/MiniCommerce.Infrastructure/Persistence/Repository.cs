using Microsoft.EntityFrameworkCore;
using MiniCommerce.Application.Common.Interfaces;
using MiniCommerce.Domain.Entities;
using MiniCommerce.Infrastructure.Persistence;

namespace MiniCommerce.Infrastructure.Persistence;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext Context;

    protected Repository(ApplicationDbContext context)
    {
        Context = context;
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await Context.Set<T>().FindAsync(new object[] { id }, cancellationToken);

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await Context.Set<T>().ToListAsync(cancellationToken);

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await Context.Set<T>().AddAsync(entity, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        Context.Set<T>().Update(entity);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        Context.Set<T>().Remove(entity);
        await Context.SaveChangesAsync(cancellationToken);
    }
}
