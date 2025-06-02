using RetailPOS.Web.Models;
using RetailPOS.Web.Models.ViewModel;

namespace RetailPOS.Web.Services.IService;

public interface IViewModelFactory
{

    RegisterViewModel CreateRegisterViewModel(RegisterViewModel? model = null);

    Task<ProductViewModel> CreateProductViewModel(ProductViewModel? model = null);

    Task<string> GetImageUrlAsync(string? imageUrl, IFormFile? image);

}
