using RetailPOS.Web.Models;

namespace RetailPOS.Web.Services.IService;

public interface IViewModelFactory
{

    RegisterViewModel CreateRegisterViewModel(RegisterViewModel? model = null);
}
