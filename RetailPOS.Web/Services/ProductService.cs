using Dapper;
using Mapster;
using RetailPOS.Web.Models;
using RetailPOS.Web.Models.ViewModel;
using RetailPOS.Web.Services.IService;
using System.Text;

namespace RetailPOS.Web.Services;

public class ProductService : IProductService
{

    private readonly IDapperBaseService _dapperBaseService;
    private readonly ILogger _logger;

    public ProductService(IDapperBaseService dapperBaseService,
        ILoggerFactory loggerFactory)
    {

        _logger = loggerFactory.CreateLogger<ProductService>();
        _dapperBaseService = dapperBaseService;
    }


    public async Task<IEnumerable<ProductViewModel>> GetProductViewModelListsAsync()
    {
        try
        {
            var query = @"
            SELECT  
                p.""Id"", 
                p.""Name"", 
                p.""Description"", 
                p.""Price"", 
                p.""StockQuantity"", 
                p.""ImageUrl"", 
                p.""CategoryId"",
                c.""Name"" AS ""CategoryName""
            FROM ""Products"" p
            LEFT JOIN ""Categories"" c ON p.""CategoryId"" = c.""Id""
            ORDER BY p.""Name"" ASC;
        ";

            return await _dapperBaseService.getDBConnectionAsync(async connection =>
                await connection.QueryAsync<ProductViewModel>(query));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product list");
            return Enumerable.Empty<ProductViewModel>();
        }
    }

    public async Task<ProductViewModel?> GetProductViewModelWithIdAsync(int? id)
    {
        try
        {
           var param = new DynamicParameters();
            param.Add("@id", id);

            var query = @"
            SELECT 
                p.""Id"",
                p.""Name"",
                p.""Description"",
                p.""Price"",
                p.""StockQuantity"",
                p.""ImageUrl"",
                p.""CategoryId"",
                c.""Name"" AS ""CategoryName""
            FROM ""Products"" p
            LEFT JOIN ""Categories"" c ON p.""CategoryId"" = c.""Id""
            WHERE p.""Id"" = @id";

            return await _dapperBaseService.getDBConnectionAsync(async connection =>
                await connection.QueryFirstOrDefaultAsync<ProductViewModel>(query, param)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving product with id: {id}");
            return null;
        }
    }
}
