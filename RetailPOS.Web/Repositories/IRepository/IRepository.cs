namespace RetailPOS.Web.Repositories.IRepository;

public interface IRepository<T> where T : class
{
    Task<int> AddAsync(T entity);

    Task<int> AddRangeAsync(IEnumerable<T> entities);

    Task<IEnumerable<T>> GetAllAsync();

    Task<T> GetByIdAsync(int id);

    Task<int> DeleteAsync(int id);

    Task<int> UpdateAsync(int id, T updateEntity);
}
