using Mapster;
using RetailPOS.Web.Models;
using RetailPOS.Web.Models.ViewModel;
using RetailPOS.Web.Repositories.IRepository;
using RetailPOS.Web.Services.IService;

namespace RetailPOS.Web.Services;

public class ProductService : IProductService
{
    private readonly IProductRepo _productRepo;
    private readonly ILogger _logger;

    public ProductService(
        IProductRepo productRepo,
        ILoggerFactory loggerFactory)
    {
        _productRepo = productRepo;
        _logger = loggerFactory.CreateLogger<ProductService>();
    }

    public async Task<ProductViewModel?> CreateProductViewModelAsync(ProductViewModel model)
    {
        try
        {
            var product = model.Adapt<Product>();
            var updatedProduct = await _productRepo.CreateProductAsync(product);

            if (updatedProduct is null) return null;

            return updatedProduct.Adapt<ProductViewModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error update product with model: {model}");
            return null;
        }
    }

    public async Task<bool> DeleteProductViewModelAsync(int id)
    {
        try
        {
            if (id is 0) return false;

            var result = await _productRepo.DeleteProductAsync(id);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error delete product with Id: {id}");
            return false;
        }
    }

    public async Task<IEnumerable<ProductViewModel>> GetProductViewModelListsAsync()
    {
        try
        {
            var result = await _productRepo.GetProductViewModelListsAsync();
            return result.Adapt<IEnumerable<ProductViewModel>>();
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
            var result = await _productRepo.GetProductViewModelWithIdAsync(id);
            return result.Adapt<ProductViewModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving product with id: {id}");
            return null;
        }
    }

    public async Task<ProductViewModel?> UpdateProductViewModelAsync(ProductViewModel model)
    {
        try
        {
            var product = model.Adapt<Product>();
            product.Id = model.Id;

            var updatedProduct = await _productRepo.UpdateProductAsync(product);

            if (updatedProduct is null) return null;

            return updatedProduct.Adapt<ProductViewModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error update product with model: {model}");
            return null;
        }

    }
}
