using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RetailPOS.Web.Services.IService;
using RetailPOS.Web.Models.ViewModel;
using RetailPOS.Web.Utility;

namespace RetailPOS.Web.Controllers;


[Authorize(Roles = $"{UserRoleConstant.Admin},{UserRoleConstant.Manager}")]
public class ProductsController : Controller
{
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IProductService _productService;
    public ProductsController(
        IViewModelFactory viewModelFactory,
        IProductService productService)
    {
        _viewModelFactory = viewModelFactory;
        _productService = productService;
    }

    #region GET: Products
    public async Task<IActionResult> Index() => View(await _productService.GetProductViewModelListsAsync());
    #endregion


    #region GET: Products/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();

        var product = await _productService.GetProductViewModelWithIdAsync(id);

        return product is null ? NotFound() : View( await _viewModelFactory.CreateProductViewModel(product));
    }
    #endregion


    #region GET: Products/Create
    public async Task<IActionResult> Create() => View(await _viewModelFactory.CreateProductViewModel());
    #endregion
    

    #region POST: Products/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductViewModel product, string? imageUrl, IFormFile? image)
    {
        if (!ModelState.IsValid) return View(await _viewModelFactory.CreateProductViewModel(product));

        product.ImageUrl = await _viewModelFactory.GetImageUrlAsync(imageUrl, image);

        var createdProduct = await _productService.CreateProductViewModelAsync(product);

        if (createdProduct is null)
        {
            ModelState.AddModelError("", "Failed to create product. Please try again.");
            return View(await _viewModelFactory.CreateProductViewModel(product));
        }

        return RedirectToAction(nameof(Index));
    }
    #endregion


    #region GET: Products/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var product = await _productService.GetProductViewModelWithIdAsync(id);

        return (product is null) ? NotFound() : View(await _viewModelFactory.CreateProductViewModel(product));
    }
    #endregion


    #region POST: Products/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductViewModel product, string? imageUrl, IFormFile? image)
    {
        if (id != product.Id) return NotFound();

        if (!ModelState.IsValid)
            return View(await _viewModelFactory.CreateProductViewModel(product));


        product.ImageUrl = await _viewModelFactory.GetImageUrlAsync(imageUrl, image);


        var UpdatedProduct = await _productService.UpdateProductViewModelAsync(product); 


        if (UpdatedProduct is null)
        {
            TempData["Error"] = "Failed to update product.";
            return View(await _viewModelFactory.CreateProductViewModel(product));
        }
        
        return RedirectToAction(nameof(Index));

    }
    #endregion


    #region GET: Products/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var productViewModel = await _productService.GetProductViewModelWithIdAsync(id);

        if (productViewModel == null)
        {
            return NotFound();
        }

        return View(productViewModel);
    }
    #endregion


    #region POST: Products/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        bool deleted = await _productService.DeleteProductViewModelAsync(id);

        if (deleted)
            return RedirectToAction(nameof(Index));

        var product = await _productService.GetProductViewModelWithIdAsync(id);
        if (product == null) return NotFound();

        TempData["Error"] = "Unable to delete product, already have transaction.";
        return View(product);
    }
    #endregion



}