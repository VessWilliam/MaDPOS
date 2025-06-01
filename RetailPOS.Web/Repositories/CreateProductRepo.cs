using Mapster;
using Microsoft.EntityFrameworkCore;
using RetailPOS.Web.Data;
using RetailPOS.Web.Models;
using RetailPOS.Web.Models.ViewModel;
using RetailPOS.Web.Repositories.IRepository;

namespace RetailPOS.Web.Repositories;

public class CreateProductRepo : Respository<Product>, ICreateProductRepo
{

    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private ILogger<CreateProductRepo> _logger;

    public CreateProductRepo(IDbContextFactory<ApplicationDbContext> contextFactory
        , ILoggerFactory loggerFactory) : base(contextFactory)
    {
        _contextFactory = contextFactory;
        _logger = loggerFactory.CreateLogger<CreateProductRepo>();
    }

    public async Task<ProductViewModel> CreateProductAsync(ProductViewModel model)
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            var product = model.Adapt<Product>();

            product.CreatedAt = DateTime.UtcNow;
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            return product.Adapt<ProductViewModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "get all contractEx id with supplier repo error");

            return null;
        }
    }
}
