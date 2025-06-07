using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetailPOS.Web.Data;
using Newtonsoft.Json;
using RetailPOS.Web.Utility;

namespace RetailPOS.Web.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context) => _context = context;

    public IActionResult Index()
    {

        var todayUtc = DateTime.UtcNow.Date;

        var totalSales = _context.SalesTransactions
            .Where(s => s.TransactionDate >= todayUtc && s.TransactionDate < todayUtc.AddDays(1)
            && s.Status == nameof(StatusEnum.COMPLETED))
            .Sum(s => (double)s.TotalAmount);

        var totalProducts = _context.Products.Count();

        var recentTransactions = _context.SalesTransactions
            .OrderByDescending(s => s.TransactionDate)
            .Take(5)
            .ToList();


        var past7DaysUtc = todayUtc.AddDays(-6);
        var groupedSales = _context.SalesTransactions
            .Where(s => s.TransactionDate >= past7DaysUtc && s.TransactionDate < todayUtc.AddDays(1) && s.Status == "COMPLETED")
            .GroupBy(s => s.TransactionDate.Date)
            .OrderBy(g => g.Key)
            .Select(g => new
            {
                Date = g.Key,
                Total = g.Sum(s => s.TotalAmount)
            })
            .ToList();


        var chartLabels = new List<string>();
        var chartValues = new List<double>();


        for (var date = past7DaysUtc; date <= todayUtc; date = date.AddDays(1))
        {
            chartLabels.Add(date.ToString("MMM dd"));
            var daySales = groupedSales.FirstOrDefault(g => g.Date == date)?.Total ?? 0;
            chartValues.Add((double)daySales);
        }

        var chartData = new
        {
            labels = chartLabels,
            data = chartValues
        };

        ViewBag.TotalSales = totalSales.ToString("N2");
        ViewBag.TotalProducts = totalProducts;
        ViewBag.RecentTransactions = recentTransactions;
        ViewBag.ChartData = JsonConvert.SerializeObject(chartData);

        return View();
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();

}
