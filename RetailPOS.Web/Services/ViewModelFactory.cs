using Microsoft.AspNetCore.Mvc.Rendering;
using RetailPOS.Web.Models;
using RetailPOS.Web.Models.ViewModel;
using RetailPOS.Web.Services.IService;
using RetailPOS.Web.Utility;

namespace RetailPOS.Web.Services;

public class ViewModelFactory : IViewModelFactory
{
    private readonly ICategoryService _categoryService;


    public ViewModelFactory(ICategoryService categoryService)
    {
        _categoryService = categoryService; 
    }


    public async Task<ProductViewModel> CreateProductViewModel(ProductViewModel? model = null)
    {

        var categories =  await _categoryService.GetCategoryAsync();

        var catergoriesList = categories.Select(c => new SelectListItem
        {

            Text = c.Name,
            Value = c.Id.ToString()

        }).ToList();

        if (model is not null) { 
          
            model.Categories = catergoriesList;
            return model;
        }

        return new ProductViewModel
        {
            Categories = catergoriesList
        };
    }

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
