using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetailPOS.Web.Data;
using RetailPOS.Web.Models;
using System.Linq;

namespace RetailPOS.Web.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        // Get today's sales
        var today = System.DateTime.Today;
        var totalSales = _context.SalesTransactions
            .Where(s => s.TransactionDate.Date == today)
            .Sum(s => (double)s.TotalAmount);

        // Get total products
        var totalProducts = _context.Products.Count();

        // Get recent transactions
        var recentTransactions = _context.SalesTransactions
            .OrderByDescending(s => s.TransactionDate)
            .Take(5)
            .ToList();

        ViewBag.TotalSales = totalSales.ToString("N2");
        ViewBag.TotalProducts = totalProducts;
        ViewBag.RecentTransactions = recentTransactions;

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
