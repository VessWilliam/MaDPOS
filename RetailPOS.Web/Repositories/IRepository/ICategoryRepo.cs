using RetailPOS.Web.Models;

namespace RetailPOS.Web.Repositories.IRepository;

public interface ICategoryRepo
{
    Task<Category?> CreateCategoryAsync(Category model);
    Task<Category?> UpdateCategoryAsync(Category model);
    Task<bool> DeleteCategoryAsync(int id);
}
