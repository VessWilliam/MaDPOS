using Dapper;
using RetailPOS.Web.Models;
using RetailPOS.Web.Services.IService;

namespace RetailPOS.Web.Services;

public class CategoryService : ICategoryService
{

    private readonly IDapperBaseService _dapperBaseService;
    private readonly ILogger _logger;


    public CategoryService(IDapperBaseService dapperBaseService,
        ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<CategoryService>();
        _dapperBaseService = dapperBaseService;
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
