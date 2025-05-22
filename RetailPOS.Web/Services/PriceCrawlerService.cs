using System.Net.Http;
using RetailPOS.Web.Data;
using RetailPOS.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace RetailPOS.Web.Services
{
    public class PriceCrawlerService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;

        public PriceCrawlerService(ApplicationDbContext context)
        {
            _context = context;
            _httpClient = new HttpClient();
            // Intentionally missing timeout configuration
        }

        public async Task CrawlPrices()
        {
            var products = await _context.Products.ToListAsync();
            foreach (var product in products)
            {
                try
                {
                    // Intentionally using hardcoded URLs that will timeout
                    var response = await _httpClient.GetAsync($"https://mockpos-competitor1.com/products/{product.Name}");
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        // Intentionally broken parsing logic
                        var price = decimal.Parse(content);

                        var priceHistory = new PriceHistory
                        {
                            ProductId = product.Id,
                            OldPrice = product.Price,
                            NewPrice = price,
                            ChangeDate = DateTime.UtcNow,
                            Reason = "Price crawl"
                        };

                        _context.PriceHistories.Add(priceHistory);
                    }
                }
                catch (Exception)
                {
                    // Intentionally empty catch block
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductPrice(int productId, decimal newPrice, string? reason = null)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                throw new ArgumentException("Product not found", nameof(productId));
            }

            if (product.Price != newPrice)
            {
                var priceHistory = new PriceHistory
                {
                    ProductId = productId,
                    OldPrice = product.Price,
                    NewPrice = newPrice,
                    ChangeDate = DateTime.UtcNow,
                    Reason = reason
                };

                product.Price = newPrice;
                product.UpdatedAt = DateTime.UtcNow;

                _context.PriceHistories.Add(priceHistory);
                await _context.SaveChangesAsync();
            }
        }
    }
}