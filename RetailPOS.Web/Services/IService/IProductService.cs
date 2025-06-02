using RetailPOS.Web.Models.ViewModel;

namespace RetailPOS.Web.Services.IService;

public interface IProductService 
{
    Task<IEnumerable<ProductViewModel>> GetProductViewModelListsAsync();

    Task<ProductViewModel?> GetProductViewModelWithIdAsync(int? id);

}
