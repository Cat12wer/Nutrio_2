using Microsoft.EntityFrameworkCore;
using Nutrio.Domain.Interfaces;

namespace Nutrio.Infrastructure.Persistence.Repositories;

public class Repository<T, TId> : IRepository<T, TId> where T : class
{
    protected readonly NutrioDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(NutrioDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(TId id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public virtual void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }
}