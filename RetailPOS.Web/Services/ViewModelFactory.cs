using Microsoft.AspNetCore.Mvc.Rendering;
using RetailPOS.Web.Models;
using RetailPOS.Web.Models.ViewModel;
using RetailPOS.Web.Services.IService;
using RetailPOS.Web.Utility;

namespace RetailPOS.Web.Services;

public class ViewModelFactory : IViewModelFactory
{
    private readonly ICategoryService _categoryService;
    private readonly IWebHostEnvironment _environment;

    public ViewModelFactory(ICategoryService categoryService, 
        IWebHostEnvironment environment)
    {
        _categoryService = categoryService;
        _environment = environment;
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

    public async Task<string> GetImageUrlAsync(string? imageUrl, IFormFile? image)
    {
        if (!string.IsNullOrWhiteSpace(imageUrl))
        {
            return imageUrl;
        }

        if (image is not null)
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid() + "_" + image.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            await using var fileStream = new FileStream(filePath, FileMode.Create);
            await image.CopyToAsync(fileStream);

            return "/uploads/" + uniqueFileName;
        }

        return "/images/placeholder.png";
    }




}
