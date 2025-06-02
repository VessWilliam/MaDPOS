using Microsoft.EntityFrameworkCore;
using RetailPOS.Web.Data;
using RetailPOS.Web.Models;
using RetailPOS.Web.Repositories.IRepository;

namespace RetailPOS.Web.Repositories;

public class ProductRepo : Respository<Product>, IProductRepo
{

    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private ILogger<ProductRepo> _logger;

    public ProductRepo(IDbContextFactory<ApplicationDbContext> contextFactory
        , ILoggerFactory loggerFactory) : base(contextFactory)
    {
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
}
