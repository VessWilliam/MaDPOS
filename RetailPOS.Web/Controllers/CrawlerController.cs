using Microsoft.AspNetCore.Mvc;
using RetailPOS.Web.Services;

namespace RetailPOS.Web.Controllers
{
    public class CrawlerController : Controller
    {
        private readonly PriceCrawlerService _crawlerService;

        public CrawlerController(PriceCrawlerService crawlerService)
        {
            _crawlerService = crawlerService;
        }

        // GET: Crawler
        public IActionResult Index()
        {
            return View();
        }

        // POST: Crawler/Start
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Start()
        {
            // Intentionally missing error handling and progress tracking
            await _crawlerService.CrawlPrices();
            return RedirectToAction(nameof(Index));
        }
    }
}