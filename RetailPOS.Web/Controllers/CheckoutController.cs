using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RetailPOS.Web.Data;
using RetailPOS.Web.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RetailPOS.Web.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CheckoutController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Checkout
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.StockQuantity > 0)
                .OrderBy(p => p.Name)
                .ToListAsync();

            return View(products);
        }

        // POST: Checkout/Process
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Process(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var transaction = new SalesTransaction
            {
                TransactionDate = DateTime.UtcNow,
                Status = "Pending",
                TotalAmount = 0
            };

            foreach (var item in model.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null || product.StockQuantity < item.Quantity)
                {
                    return BadRequest("Invalid product or insufficient stock");
                }

                var transactionItem = new SalesTransactionItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price,
                    TotalPrice = product.Price * item.Quantity
                };

                transaction.Items.Add(transactionItem);
                transaction.TotalAmount += transactionItem.TotalPrice;

                // Update stock
                product.StockQuantity -= item.Quantity;
            }

            _context.SalesTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Receipt), new { id = transaction.Id });
        }

        // GET: Checkout/Receipt/5
        public async Task<IActionResult> Receipt(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.SalesTransactions
                .Include(s => s.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }
    }

    public class CheckoutViewModel
    {
        public List<CheckoutItem> Items { get; set; } = new List<CheckoutItem>();
    }

    public class CheckoutItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}