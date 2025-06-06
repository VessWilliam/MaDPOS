using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RetailPOS.Web.Data;
using RetailPOS.Web.Models;
using RetailPOS.Web.Utility;
using RetailPOS.Web.Repositories.IRepository;
using RetailPOS.Web.Services.IService;

namespace RetailPOS.Web.Controllers;

[Authorize(Roles = $"{UserRoleConstant.Admin},{UserRoleConstant.Manager}, {UserRoleConstant.Cashier}")]
public class SalesController : Controller
{
    private readonly ApplicationDbContext _context;
    private ISaleTransactionsService _saleTransactionsService;
    private IProductService _productService;

    public SalesController(ApplicationDbContext context,
        IProductService productService,
        ISaleTransactionsService saleTransactionsService)
    {
        _context = context;
        _productService = productService;
        _saleTransactionsService = saleTransactionsService;
    }

    // GET: Sales
    public async Task<IActionResult> Index()
    {
        var sales = await _context.SalesTransactions
            .Include(s => s.Items)
                .ThenInclude(i => i.Product)
            .OrderByDescending(s => s.TransactionDate)
            .ToListAsync();

        var totalSales = sales.Sum(s => s.TotalAmount);
        var totalTransactions = sales.Count;
        var averageTransaction = totalTransactions > 0 ? totalSales / totalTransactions : 0;

        
        ViewBag.TotalSales = totalSales;
        ViewBag.TotalTransactions = totalTransactions;
        ViewBag.AverageTransaction = averageTransaction;

        return View(sales);
    }

    // GET: Sales/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var sale = await _context.SalesTransactions
            .Include(s => s.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sale == null)
        {
            return NotFound();
        }

        return View(sale);
    }

    // GET: Sales/Create
    public IActionResult Create()
    {
        ViewBag.Products = _context.Products
            .Where(p => p.StockQuantity > 0)
            .OrderBy(p => p.Name)
            .ToList();
        return View();
    }

    // POST: Sales/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SalesTransaction sale)
    {
        if (ModelState.IsValid)
        {
            sale.TransactionDate = DateTime.UtcNow;
            sale.Status = "Pending";
            _context.Add(sale);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Products = _context.Products
            .Where(p => p.StockQuantity > 0)
            .OrderBy(p => p.Name)
            .ToList();
        return View(sale);
    }

    // GET: Sales/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();
        

        var sale = await _saleTransactionsService.GetTransactionWithItemsIdAsync(id.Value);


        if (sale is null) return NotFound();
        

        if (sale.Status is nameof(StatusEnum.COMPLETED))  return RedirectToAction(nameof(Index));
        

        ViewBag.Products = sale.Items
            .Where(i => i.Product != null)
            .Select(i => i.Product)
            .ToList()
            .OrderBy(i => i!.Name);

        return View(sale);
    }

    // POST: Sales/Edit/5
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

    // GET: Sales/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var sale = await _context.SalesTransactions
            .Include(s => s.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sale == null)
        {
            return NotFound();
        }

        if (sale.Status == "Completed")
        {
            return RedirectToAction(nameof(Index));
        }

        return View(sale);
    }

    // POST: Sales/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var sale = await _context.SalesTransactions
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sale == null)
        {
            return NotFound();
        }

        if (sale.Status == "Completed")
        {
            return RedirectToAction(nameof(Index));
        }

        // Restore stock quantities
        foreach (var item in sale.Items)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product != null)
            {
                product.StockQuantity += item.Quantity;
            }
        }

        _context.SalesTransactions.Remove(sale);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool SaleExists(int id)
    {
        return _context.SalesTransactions.Any(e => e.Id == id);
    }

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