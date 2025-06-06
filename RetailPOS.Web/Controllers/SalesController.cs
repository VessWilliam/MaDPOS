using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RetailPOS.Web.Data;
using RetailPOS.Web.Models;
using RetailPOS.Web.Utility;
using RetailPOS.Web.Services.IService;
using Mapster;
using RetailPOS.Web.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace RetailPOS.Web.Controllers;

[Authorize(Roles = $"{UserRoleConstant.Admin},{UserRoleConstant.Manager}, {UserRoleConstant.Cashier}")]
public class SalesController : Controller
{
    private readonly ApplicationDbContext _context;
    private ISaleTransactionsService _saleTransactionsService;
    private IProductService _productService;
    private readonly UserManager<ApplicationUser> _userManager;
    public SalesController(ApplicationDbContext context,
        IProductService productService,
        UserManager<ApplicationUser> userManager,
        ISaleTransactionsService saleTransactionsService)
    {
        _context = context;
        _productService = productService;
        _userManager = userManager;
        _saleTransactionsService = saleTransactionsService;
    }

    #region GET: Sales
    public async Task<IActionResult> Index()
    {
        var sales = await _saleTransactionsService.GetTransactionsWithItemsAsync();

        var totalSales = sales.Sum(s => s.TotalAmount);
        var totalTransactions = sales.Count;
        var averageTransaction = totalTransactions > 0 ? totalSales / totalTransactions : 0;

        ViewBag.TotalSales = totalSales;
        ViewBag.TotalTransactions = totalTransactions;
        ViewBag.AverageTransaction = averageTransaction;
        return View(sales);
    }
    #endregion


    #region GET: Sales/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();


        var sale = await _saleTransactionsService.GetTransactionWithItemsIdAsync(id.Value);


        if (sale is null) return NotFound();


        if (sale.Status is nameof(StatusEnum.COMPLETED)) return RedirectToAction(nameof(Index));


        ViewBag.Products = sale.Items
            .Where(i => i.Product != null)
            .Select(i => i.Product)
            .ToList()
            .OrderBy(i => i!.Name);

        return View(sale);
    }
    #endregion


    #region POST: Sales/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SalesTransaction sale)
    {
        if (id != sale.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(sale);

        var updated = await _saleTransactionsService
            .UpdateTransactionStatusAsync(id, sale.Status, sale.PaymentStatus!);

        if (!updated)
            return RedirectToAction(nameof(Index));

        return RedirectToAction(nameof(Index));
    }
    #endregion


    #region GET: Sales/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();

        var sale = await _saleTransactionsService
            .GetTransactionWithItemsIdAsync(id.Value);

        return sale is null ? NotFound() : View(sale);
    }
    #endregion


    #region GET: Sales/Create
    public async Task<IActionResult> Create()
    {
        var products = await _productService.GetCheckOutProductListAsync();

        ViewBag.Products = products.Adapt<List<ProductViewModel>>()
            .OrderBy(i => i.Name);

        return View();
    }
    #endregion


    #region POST: Sales/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SalesTransaction sale)
    {
        if (!ModelState.IsValid)
        {
            var products = await _productService.GetCheckOutProductListAsync();

            ViewBag.Products = products.Adapt<List<ProductViewModel>>()
                .OrderBy(i => i.Name);

            return View(sale);
        }



        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId!);
        if (user == null) return Unauthorized();

        // Set user info into the incoming model
        sale.UserId = userId;
        sale.CustomerName = $"{user.FirstName} {user.LastName}";
        sale.CustomerEmail = user.Email;
        sale.TransactionDate = DateTime.UtcNow;
        sale.Status = nameof(StatusEnum.PENDING);
        sale.PaymentMethod = "SALE_CREATE";
        sale.PaymentStatus = nameof(StatusEnum.PENDING);

        await _saleTransactionsService.CreateNewSaleTransaction(sale);

        return RedirectToAction(nameof(Index));
    }
    #endregion


    #region GET: Sales/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
            return NotFound();

        var result = await _saleTransactionsService.GetTransactionsWithByIdAsync(id.Value);

        if (result is null)
            return NotFound();

        if (result.Status is nameof(StatusEnum.COMPLETED))
        {
            TempData["Error"] = "Completed transactions cannot be deleted.";
            return RedirectToAction(nameof(Index));
        }

        return View(result);
    }
    #endregion


    #region POST: Sales/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var success = await _saleTransactionsService.DeleteSaleTransactionAsync(id);

        if (!success)
        {
            TempData["Error"] = "Unable to delete transaction. It may not exist or is already completed.";
            return RedirectToAction(nameof(Index));
        }

        TempData["Success"] = "Sale transaction deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
    #endregion

    // GET: Sales/Report
    public async Task<IActionResult> Report(DateTime? startDate, DateTime? endDate)
    {
        var query = _context.SalesTransactions
            .Include(s => s.Items)
                .ThenInclude(i => i.Product)
            .AsQueryable();

        if (startDate.HasValue)
        {
            query = query.Where(s => s.TransactionDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(s => s.TransactionDate <= endDate.Value);
        }

        var sales = await query.ToListAsync();
        return View(sales);
    }
}