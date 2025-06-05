using RetailPOS.Web.Models;

namespace RetailPOS.Web.Services.IService;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetCategoryAsync();
    Task<Category?> CreateCategoryAsync(Category model);
    Task<Category?> UpdateCategoryAsync(Category model);
    Task<bool> DeleteCategoryAsync(int id);
    Task<Category?> GetCategoryByIdAsync(int id);



    #region CategoryProduct
    Task<IEnumerable<Category>> GetCategoriesWithProductsAsync();
    Task<Category?> GetCategoryWithProductsByIdAsync(int id);
    Task<(bool success, string error)> DeleteConfirmService(int id);
    #endregion
}
