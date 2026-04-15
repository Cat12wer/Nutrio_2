namespace Nutrio.Domain.Interfaces;

public interface IRepository<T, TId> where T : class
{
    Task<T?> GetByIdAsync(TId id);
    Task<IReadOnlyList<T>> GetAllAsync();
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}