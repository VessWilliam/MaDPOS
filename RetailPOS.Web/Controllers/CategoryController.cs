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

    // POST: Category/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Category category)
    {
        if (id != category.Id)  return NotFound();

        if (!ModelState.IsValid) return View(category);

        await _categoryService.UpdateCategoryAsync(category); 
        
        return RedirectToAction(nameof(Index));
    }

    // GET: Category/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        if (id is 0)  return NotFound();

        var category = await _categoryService.GetCategoryWithProductsByIdAsync(id);
        
        return category is null ? NotFound() :View(category);      
       
    }

    // POST: Category/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category is null)  return NotFound();
        

        if (category.Products.Any())
        {
            ModelState.AddModelError("", "Cannot delete category with associated products.");
            return View("Delete", category);
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool CategoryExists(int id)
    {
        return _context.Categories.Any(e => e.Id == id);
    }
}