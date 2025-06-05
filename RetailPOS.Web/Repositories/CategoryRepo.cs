using Dapper;
using Microsoft.EntityFrameworkCore;
using RetailPOS.Web.Data;
using RetailPOS.Web.Models;
using RetailPOS.Web.Repositories.IRepository;
using RetailPOS.Web.Services.IService;

namespace RetailPOS.Web.Repositories;

public class CategoryRepo : Respository<Category>, ICategoryRepo
{

    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly IDapperBaseService _dapperBaseService;
    private readonly ILogger<CategoryRepo> _logger;

    public CategoryRepo(
        IDapperBaseService dapperBaseService,
        IDbContextFactory<ApplicationDbContext> contextFactory,
        ILoggerFactory loggerFactory
    ) : base(contextFactory)
    {
        _dapperBaseService = dapperBaseService;
        _contextFactory = contextFactory;
        _logger = loggerFactory.CreateLogger<CategoryRepo>();
    }


    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        try
        {
            var query = @"SELECT * FROM ""Categories"" WHERE ""Id"" = @Id";

            return await _dapperBaseService.getDBConnectionAsync(async connection =>
                await connection.QueryFirstOrDefaultAsync<Category>(query, new { Id = id }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to get category by Id {id}");
            return null;
        }
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

    public async Task<Category?> GetCategoryWithProductsByIdAsync(int id)
    {
        var query = @"
        SELECT c.*, p.* 
        FROM ""Categories"" c
        LEFT JOIN ""Products"" p ON c.""Id"" = p.""CategoryId""
        WHERE c.""Id"" = @Id";

        var categoryDict = new Dictionary<int, Category>();

        await _dapperBaseService.getDBConnectionAsync(async connection =>
        await connection.QueryAsync<Category, Product, Category>(
                query,
                (category, product) =>
                {
                    if (!categoryDict.TryGetValue(category.Id, out var currentCategory))
                    {
                        currentCategory = category;
                        currentCategory.Products = new List<Product>();
                        categoryDict[category.Id] = currentCategory;
                    }

                    if (product is not null)
                        currentCategory.Products.Add(product);

                    return currentCategory;
                },
                new { Id = id },
                splitOn: "Id"
            )
        );

        return categoryDict.Values.FirstOrDefault();
    }

    public async Task<IEnumerable<Category>> GetCategoriesWithProductsAsync()
    {

        var query = @"
        SELECT c.*, p.* 
        FROM ""Categories"" c
        LEFT JOIN ""Products"" p ON c.""Id"" = p.""CategoryId""
        ORDER BY c.""Id""";

        var categoryDict = new Dictionary<int, Category>();

        var result = await _dapperBaseService.getDBConnectionAsync(async connection =>
            await connection.QueryAsync<Category, Product, Category>(
                query,
                (category, product) =>
                {
                    if (!categoryDict.TryGetValue(category.Id, out var currentCategory))
                    {
                        currentCategory = category;
                        currentCategory.Products = new List<Product>();
                        categoryDict[category.Id] = currentCategory;
                    }

                    if (product is not null)
                        currentCategory.Products.Add(product);

                    return currentCategory;
                },
                splitOn: "Id"
            )
        );

        return categoryDict.Values;
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

    public async Task<IEnumerable<Category>> GetCategoryAsync()
    {
        try
        {
            var query = @" Select * From ""Categories"" Order By ""Id"" ASC";

            return await _dapperBaseService.getDBConnectionAsync(async connection =>
            await connection.QueryAsync<Category>(query));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Get Category list Error {ex}");
            return Enumerable.Empty<Category>();
        }
    }
}