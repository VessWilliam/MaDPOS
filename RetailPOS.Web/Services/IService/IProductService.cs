using RetailPOS.Web.Models;
using RetailPOS.Web.Models.ViewModel;

namespace RetailPOS.Web.Services.IService;

public interface IProductService 
{
    Task<IEnumerable<ProductViewModel>> GetProductViewModelListsAsync();

    Task<ProductViewModel?> GetProductViewModelWithIdAsync(int? id);

    Task<ProductViewModel?> UpdateProductViewModelAsync(ProductViewModel model);

    Task<ProductViewModel?> CreateProductViewModelAsync(ProductViewModel model);

    Task<bool> DeleteProductViewModelAsync(int id);

    Task<IEnumerable<ProductViewModel>?> GetCheckOutProductListAsync();
}
