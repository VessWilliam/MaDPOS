namespace RetailPOS.Web.Repositories.IRepository;

public interface IRepository<T> where T : class
{
    Task<int> AddAsync(T entity);

    Task<int> AddRangeAsync(IEnumerable<T> entities);

    Task<int> DeleteAsync(T deleteEntity);

    Task<IEnumerable<T>> GetAllAsync();

    Task<T> GetByIdAsync(int id);

    Task<int> UpdateAsync(T updatedEntity);
}
