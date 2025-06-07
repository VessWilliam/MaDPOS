using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RetailPOS.Web.Models;
using RetailPOS.Web.Services.IService;
using RetailPOS.Web.Models.ViewModel;
using RetailPOS.Web.Utility;
using Mapster;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace RetailPOS.Web.Controllers
{
    [Authorize(Roles = $"{UserRoleConstant.Admin},{UserRoleConstant.Manager}, {UserRoleConstant.Cashier}")]
    public class CheckoutController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IViewModelFactory _viewModelFactory;
        private readonly IRedisCacheService _redisCacheService;
        private readonly IProductService _productService;
        private readonly ISaleTransactionsService _saleTransactionsService;

        public CheckoutController(
            IProductService productService,
            IViewModelFactory viewModelFactory,
            UserManager<ApplicationUser> userManager,
            IRedisCacheService redisCacheService,
            ISaleTransactionsService saleTransactionsService)
        {
            _productService = productService;
            _viewModelFactory = viewModelFactory;
            _userManager = userManager;
            _redisCacheService = redisCacheService;
            _saleTransactionsService = saleTransactionsService;
        }

        // POST: Checkout/Process
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(string paymentMethod, decimal amount)
        {
            try
            {
                var cart = await _redisCacheService.GetData<List<CartItemViewModel>>(_redisCacheService
                    .GetOrCreateCartId());

                if (cart == null || !cart.Any()) return BadRequest("Cart is empty.");

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId!);
                if (user is null) return Unauthorized();

                var transaction = new SalesTransaction
                {
                    TransactionDate = DateTime.UtcNow,
                    Status = nameof(StatusEnum.PENDING),
                    PaymentMethod = paymentMethod,
                    PaymentStatus = nameof(StatusEnum.PENDING),
                    CustomerName = $"{user.FirstName} {user.LastName}",
                    CustomerEmail = user.Email,
                    UserId = userId,
                    Items = new List<SalesTransactionItem>()
                };

                var transactionId = await _saleTransactionsService.ProcessTransactionAsync(transaction, cart, amount);


                if (transactionId is 0)
                {
                    TempData["Error"] = "Transaction failed. Please try again.";
                    ModelState.AddModelError("", "Transaction failed. Please try again.");
                    return View("Payment");
                }


                await _redisCacheService.RemoveData(_redisCacheService.GetOrCreateCartId());
                return RedirectToAction(nameof(Receipt), new { id = transactionId });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }




        // GET: Checkout
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetCheckOutProductListAsync();

            var cart = await _redisCacheService
                .GetData<List<CartItemViewModel>>(_redisCacheService.GetOrCreateCartId()) ?? new();

            var model = new CheckoutViewModel
            {
                Products = products.Adapt<List<ProductViewModel>>(),
                CartItems = cart
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveCart([FromBody] List<CartItemViewModel> cart)
        {
            await _redisCacheService.SetData(_redisCacheService.GetOrCreateCartId(), cart, TimeSpan.FromMinutes(30));
            return Ok();
        }


        // GET: Checkout/Payment
        public async Task<IActionResult> Payment()
        {
            var cart = await _redisCacheService.GetData<List<CartItemViewModel>>(_redisCacheService.GetOrCreateCartId()) ?? new();

            if (!cart.Any())
            {
                TempData["Error"] = "Cart is empty. Add items before proceeding to payment.";
                return RedirectToAction(nameof(Index));
            }

            var total = cart.Sum(item => item.Price * item.Quantity);
            ViewBag.Total = total;

            return View();
        }



        // GET: Checkout/Receipt/5
        public async Task<IActionResult> Receipt(int? id)
        {
            if (id is null) return NotFound();

            var result = await _saleTransactionsService
                .GetTransactionWithItemsIdAsync(id.Value);

            return result is null ? NotFound() : View(result);

        }
    }
}