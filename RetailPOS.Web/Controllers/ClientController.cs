using Microsoft.AspNetCore.Mvc;
using RetailPOS.Web.Data;
using RetailPOS.Web.Models.ViewModel;
using RetailPOS.Web.Services.IService;
using RetailPOS.Web.Utility;

namespace RetailPOS.Web.Controllers;

public class ClientController : Controller
{

    private readonly IProductService _productService;
    private readonly IRedisCacheService _redisCacheService;
    private readonly ApplicationDbContext _context;
    public ClientController(IProductService productService,
        ApplicationDbContext context,
        IRedisCacheService redisCacheService)
    {
        _productService = productService;
        _context = context;
        _redisCacheService = redisCacheService;
    }

    public async Task<IActionResult> Index()
    {
        if (User.Identity!.IsAuthenticated)
        {
            if (User.IsInRole(nameof(UserRoleConstant.Cashier))
                || User.IsInRole(nameof(UserRoleConstant.Manager)))
            {
                return RedirectToAction(nameof(Index), "Home");
            }
        }

        return View(await _productService.GetProductViewModelListsAsync());
    }

    public async Task<IActionResult> Cart()
    {
        var cart = await _redisCacheService.GetData<List<CartItemViewModel>>(nameof(CartItemViewModel)) ?? new();
        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(int id)
    {
        var product = await _productService.GetProductViewModelWithIdAsync(id);
        if (product is null) return NotFound();

        var cart = await _redisCacheService.GetData<List<CartItemViewModel>>(nameof(CartItemViewModel)) ?? new();

        var item = cart.FirstOrDefault(c => c.ProductId == id);
        if (item != null)
        {
            item.Quantity += 1;
        }
        else
        {
            cart.Add(new CartItemViewModel
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                Quantity = 1,
                imageUrl = product.ImageUrl
            });
        }

        await _redisCacheService.SetData(nameof(CartItemViewModel), cart, TimeSpan.FromMinutes(30));
        return RedirectToAction(nameof(Cart));
    }

    [HttpPost]
    public async Task<IActionResult> UpdateCart(int ProductId, int Quantity)
    {

        var cart = await _redisCacheService.GetData<List<CartItemViewModel>>(nameof(CartItemViewModel)) ?? new();


        var item = cart.FirstOrDefault(c => c.ProductId == ProductId);
        if (item is not null)
        {
            if (Quantity <= 0)
            {
                cart.Remove(item);
            }
            else
            {
                item.Quantity = Quantity;
            }

            await _redisCacheService.SetData(nameof(CartItemViewModel), cart, TimeSpan.FromMinutes(30));

        }

        return RedirectToAction(nameof(Cart));
    }


    [HttpPost]
    public async Task<IActionResult> RemoveCart(int ProductId)
    {
        var cart = await _redisCacheService.GetData<List<CartItemViewModel>>(nameof(Cart)) ?? new();

        var item = cart.FirstOrDefault(c => c.ProductId == ProductId);
        if (item is not null)
            cart.Remove(item);


        await _redisCacheService.SetData(nameof(CartItemViewModel), cart, TimeSpan.FromMinutes(30));


        return RedirectToAction(nameof(Index));
    }


    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var cart = await _redisCacheService.GetData<List<CartItemViewModel>>(nameof(Cart)) ?? new();

        var viewModel = new CheckoutViewModel
        {
            CartItems = cart
        };

        return View(viewModel);
    }


    [HttpPost]
    public IActionResult ProceedToCheckout() => RedirectToAction("Index", "Checkout");



}
