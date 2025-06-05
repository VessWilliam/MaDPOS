using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RetailPOS.Web.Data;
using RetailPOS.Web.Models;
using RetailPOS.Web.Utility;
using RetailPOS.Web.Services.IService;

namespace RetailPOS.Web.Controllers;

[Authorize(Roles = $"{UserRoleConstant.Admin},{UserRoleConstant.Manager}")]
public class CategoryController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ICategoryService _categoryService;

    public CategoryController(ApplicationDbContext context, ICategoryService categoryService)
    {
        _context = context;
        _categoryService = categoryService;
    }

    #region GET: Category
    public async Task<IActionResult> Index() =>
        View(await _categoryService.GetCategoriesWithProductsAsync());
    #endregion



    #region GET: Category/Details/5
    public async Task<IActionResult> Details(int id)
    {
        if (id is 0)
            return NotFound();

        var category = await _categoryService.GetCategoryWithProductsByIdAsync(id);

         return category is null ? NotFound() : View(category);
    }
    #endregion



    #region GET: Category/Create
    public IActionResult Create() => View();
    #endregion




    #region POST: Category/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Description")] Category category)
    {
        if (!ModelState.IsValid)  return View(category);

        await _categoryService.CreateCategoryAsync(category); 

        return RedirectToAction(nameof(Index));
    }
    #endregion



    #region GET: Category/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        if (id is 0) return NotFound();

        var category =  await _categoryService.GetCategoryByIdAsync(id);

        return category is null ? NotFound() : View(category);
    }
    #endregion

    #region POST: Category/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Category category)
    {
        if (id != category.Id)  return NotFound();

        if (!ModelState.IsValid) return View(category);

        await _categoryService.UpdateCategoryAsync(category); 
        
        return RedirectToAction(nameof(Index));
    }
    #endregion

    #region GET: Category/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        if (id is 0)  return NotFound();

        var category = await _categoryService.GetCategoryWithProductsByIdAsync(id);
        
        return category is null ? NotFound() :View(category);      
       
    }
    #endregion

    #region POST: Category/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var (success, error, category) = await _categoryService.DeleteConfirmAsync(id);

        if (success)
        {
            TempData["Success"] = "Category deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        if (error is "Category not found") return NotFound();

        if (category is not null)
        {
            ModelState.AddModelError("", error ?? "An error occurred while deleting.");
            return View("Delete", category);
        }

        TempData["Error"] = error ?? "Unexpected error.";
        return RedirectToAction(nameof(Index));
    }

    #endregion
}