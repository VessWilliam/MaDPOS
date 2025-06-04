using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RetailPOS.Web.Data;
using RetailPOS.Web.Models;
using RetailPOS.Web.Services.IService;
using RetailPOS.Web.Models.ViewModel;

namespace RetailPOS.Web.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IViewModelFactory _viewModelFactory;   
        private readonly IRedisCacheService _redisCacheService; 

        public CheckoutController(ApplicationDbContext context,  IViewModelFactory viewModelFactory, 
            IRedisCacheService redisCacheService)
        {
            _context = context;
            _viewModelFactory = viewModelFactory;
            _redisCacheService = redisCacheService; 
        }

        // GET: Checkout
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.StockQuantity > 0)
                .OrderBy(p => p.Name)
                .Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    ImageUrl = p.ImageUrl
                })
            .ToListAsync();

            var cart = await _redisCacheService.GetData<List<CartItemViewModel>>(nameof(CartItemViewModel)) ?? new();

            var model = new CheckoutViewModel
            {
                Products = products,
                CartItems = cart
            };

            return View(model);
        }


        // POST: Checkout/Process
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Process([FromBody] CheckoutViewModel model)
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

            return Json(new { redirectUrl = Url.Action(nameof(Receipt), new { id = transaction.Id }) });
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



  
}