using RetailPOS.Web.Models;
using RetailPOS.Web.Repositories.IRepository;
using RetailPOS.Web.Services.IService;

namespace RetailPOS.Web.Services;

public class CategoryService : ICategoryService
{

    private readonly ICategoryRepo _categoryRepo;
    private readonly ILogger _logger;


    public CategoryService(IDapperBaseService dapperBaseService,
        ICategoryRepo categoryRepo,
        ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<CategoryService>();
        _categoryRepo = categoryRepo;
    }

    public async Task<Category?> CreateCategoryAsync(Category model)
    {
        try
        {
            var result = await _categoryRepo.CreateCategoryAsync(model);

            if (result is null) return null;

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error delete  {nameof(Category)}  with Id: : {model}");
            return null;
        }
    }

    public async Task<IEnumerable<Category>> GetCategoryAsync()
    {
        try
        {
            return await _categoryRepo.GetCategoryAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Get Category list Error {ex}");
            return Enumerable.Empty<Category>();
        }
    }

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        try
        {
            return await _categoryRepo.GetCategoryByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to get category by Id {id}");
            return null;
        }
    }

    public async Task<Category?> UpdateCategoryAsync(Category model)
    {
        try
        {
            var updateCategory = await _categoryRepo.UpdateCategoryAsync(model);

            if (updateCategory is null) return null;

            return updateCategory;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error update with model: {model}");
            return null;
        }

    }

    public async Task<IEnumerable<Category>> GetCategoriesWithProductsAsync()
    {
        try
        {
            return await _categoryRepo.GetCategoriesWithProductsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to get category with product");
            return null;
        }

    }


    public async Task<Category?> GetCategoryWithProductsByIdAsync(int id)
    {
        try
        {
            var result = await _categoryRepo.GetCategoryWithProductsByIdAsync(id);

            return result is null ? null : result;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting category with id model: {id}");
            return null;
        }
    }

    public async Task<(bool success, string? error, Category? category)> DeleteConfirmAsync(int id)
    {
        try
        {
            var category = await _categoryRepo.GetCategoryWithProductsByIdAsync(id);

            if (category is null)
                return (false, "Category not found", null);

            if (category.Products.Any())
                return (false, "Cannot delete category with associated products.", category);

            var result = await _categoryRepo.DeleteCategoryAsync(category.Id);
            return result
                ? (true, null, null)
                : (false, "Failed to delete category", category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting {nameof(Category)} with Id: {id}");
            return (false, "Unexpected error occurred", null);
        }
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        try
        {
            if (id is 0) return false;

            var result = await _categoryRepo.DeleteCategoryAsync(id);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error delete {nameof(Category)} with Id: {id}");
            return false;
        }
    }
}
