using RetailPOS.Web.Models.ViewModel;

namespace RetailPOS.Web.Repositories.IRepository;

public interface ICreateProductRepo
{
  Task<ProductViewModel> CreateProductAsync(ProductViewModel model);
}
