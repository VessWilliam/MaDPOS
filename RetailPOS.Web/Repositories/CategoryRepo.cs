using Microsoft.EntityFrameworkCore;
using RetailPOS.Web.Data;
using RetailPOS.Web.Models;
using RetailPOS.Web.Repositories.IRepository;

namespace RetailPOS.Web.Repositories;

public class CategoryRepo : Respository<Category>, ICategoryRepo
{
    private readonly ILogger<CategoryRepo> _logger;

    public CategoryRepo(
        IDbContextFactory<ApplicationDbContext> contextFactory,
        ILoggerFactory loggerFactory
    ) : base(contextFactory)
    {
        _logger = loggerFactory.CreateLogger<CategoryRepo>();
    }

    public async Task<Category?> CreateCategoryAsync(Category model)
    {

        try
        {
            model.CreatedAt = DateTime.UtcNow;

            var rowsAffected = await AddAsync(model);

            return rowsAffected is 0 ? null : model;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating {nameof(Category)} ");
            return null;
        }



    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        try
        {
            var rowsAffected = await DeleteAsync(id);

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting {nameof(Category)}");
            return false;
        }
    }

    public async Task<Category?> UpdateCategoryAsync(Category model)
    {
        try
        {
            model.UpdatedAt = DateTime.UtcNow;

            var rowsAffected = await UpdateAsync(model.Id, model);

            return rowsAffected is 0 ? null : model;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating {nameof(Category)}");
            return null;
        }
    }
}