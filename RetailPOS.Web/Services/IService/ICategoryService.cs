using RetailPOS.Web.Models;

namespace RetailPOS.Web.Services.IService;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetCategoryAsync();
    Task<Category?> UpdateCategoryAsync(Category model);
    
    Task<Category?> GetCategoryByIdAsync(int id);

    Task<Category?> CreateCategoryAsync(Category model);


 
    Task<IEnumerable<Category>> GetCategoriesWithProductsAsync();
    Task<Category?> GetCategoryWithProductsByIdAsync(int id);
    Task<(bool success, string? error, Category? category)> DeleteConfirmAsync(int id);
   
}
