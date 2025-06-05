using Dapper;
using RetailPOS.Web.Models;
using RetailPOS.Web.Repositories.IRepository;
using RetailPOS.Web.Services.IService;

namespace RetailPOS.Web.Services;

public class CategoryService : ICategoryService
{

    private readonly IDapperBaseService _dapperBaseService;
    private readonly ICategoryRepo _categoryRepo;
    private readonly ILogger _logger;


    public CategoryService(IDapperBaseService dapperBaseService,
        ICategoryRepo categoryRepo,
        ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<CategoryService>();
        _dapperBaseService = dapperBaseService;
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

}
