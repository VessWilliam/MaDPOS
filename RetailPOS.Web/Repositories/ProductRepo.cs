using Dapper;
using Microsoft.EntityFrameworkCore;
using RetailPOS.Web.Data;
using RetailPOS.Web.Models;
using RetailPOS.Web.Repositories.IRepository;
using RetailPOS.Web.Services.IService;

namespace RetailPOS.Web.Repositories;

public class ProductRepo : Respository<Product>, IProductRepo
{
    private readonly IDapperBaseService _dapperBaseService;
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private ILogger<ProductRepo> _logger;

    public ProductRepo(IDapperBaseService dapperBaseService,
           IDbContextFactory<ApplicationDbContext> contextFactory
        , ILoggerFactory loggerFactory) : base(contextFactory)
    {
        _dapperBaseService = dapperBaseService;
        _contextFactory = contextFactory;
        _logger = loggerFactory.CreateLogger<ProductRepo>();
    }


    public async Task<Product?> CreateProductAsync(Product model)
    {
        try
        {
            model.CreatedAt = DateTime.UtcNow;

            var rowsAffected = await AddAsync(model);

            return rowsAffected is 0 ? null : model;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return null;
        }
    }

    public async Task<Product?> UpdateProductAsync(Product model)
    {
        try
        {
            model.UpdatedAt = DateTime.UtcNow;

            var rowsAffected = await UpdateAsync(model.Id, model);

            return rowsAffected is 0 ? null : model;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product");
            return null;
        }
    }

    public async Task<bool> DeleteProductAsync(int Id)
    {
        try
        {
            var rowsAffected = await DeleteAsync(Id);

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product");
            return false;
        }
    }

    public async Task<Product?> GetProductViewModelWithIdAsync(int? id)
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
                await connection.QueryFirstOrDefaultAsync<Product>(query, param)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving product with id: {id}");
            return null;
        }
    }

    public async Task<IEnumerable<Product>> GetProductViewModelListsAsync()
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
                await connection.QueryAsync<Product>(query));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product list");
            return Enumerable.Empty<Product>();
        }
    }
}
