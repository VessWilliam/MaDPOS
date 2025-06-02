using Microsoft.EntityFrameworkCore;
using RetailPOS.Web.Data;
using RetailPOS.Web.Repositories.IRepository;

namespace RetailPOS.Web.Repositories;

public class Respository<T> : IRepository<T> where T : class
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public Respository(IDbContextFactory<ApplicationDbContext> contextFactory) => _contextFactory = contextFactory;

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Set<T>().ToListAsync();
    }

    public async Task<T> GetByIdAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Set<T>().FindAsync(id);
    }

    public async Task<int> AddAsync(T entity)
    {
        using var context = _contextFactory.CreateDbContext();
        await context.Set<T>().AddAsync(entity);
        return await context.SaveChangesAsync();
    }

    public async Task<int> AddRangeAsync(IEnumerable<T> entities)
    {
        using var context = _contextFactory.CreateDbContext();
        await context.Set<T>().AddRangeAsync(entities);
        return await context.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();

        var existingEntity = await context.Set<T>().FindAsync(id);

        if (existingEntity is null) return 0;

        context.Set<T>().Remove(existingEntity);

        return await context.SaveChangesAsync();
    }

    public async Task<int> UpdateAsync(int id, T updatedEntity)
    {
        using var context = _contextFactory.CreateDbContext();

        var existingEntity = await context.Set<T>().FindAsync(id);

        if (existingEntity is null) return 0;

        context.Entry(existingEntity).CurrentValues.SetValues(updatedEntity);

        return await context.SaveChangesAsync();
    }
}
