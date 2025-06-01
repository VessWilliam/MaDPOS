using Microsoft.AspNetCore.Mvc.Rendering;
using RetailPOS.Web.Models;
using RetailPOS.Web.Services.IService;
using RetailPOS.Web.Utility;

namespace RetailPOS.Web.Services;

public class ViewModelFactory : IViewModelFactory
{
    public RegisterViewModel CreateRegisterViewModel(RegisterViewModel? model = null)
    {

        var roles = new List<SelectListItem>
        {
            new() { Text = nameof(UserRoleConstant.Admin), Value = UserRoleConstant.Admin },
            new() { Text = nameof(UserRoleConstant.Manager), Value = UserRoleConstant.Manager },
            new() { Text = nameof(UserRoleConstant.Cashier), Value = UserRoleConstant.Cashier }

        };

        if (model is not null)
        {
            model.Roles = roles;
            return model;
        }
        return new RegisterViewModel { Roles = roles };
    }
}
