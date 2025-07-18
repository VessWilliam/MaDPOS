﻿using Microsoft.AspNetCore.Mvc;
using RetailPOS.Web.Models.ViewModel;
using RetailPOS.Web.Services.IService;
using RetailPOS.Web.Utility;

namespace RetailPOS.Web.Controllers;

public class ClientController : Controller
{

    private readonly IProductService _productService;
    private readonly IRedisCacheService _redisCacheService;

    public ClientController(IProductService productService,
        IRedisCacheService redisCacheService)
    {
        _productService = productService;
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
        var cartId = _redisCacheService.GetOrCreateCartId();
        var cart = await _redisCacheService.GetData<List<CartItemViewModel>>(cartId) ?? new();
        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(int id)
    {
        var product = await _productService.GetProductViewModelWithIdAsync(id);
        if (product is null) return NotFound();

        var cartId = _redisCacheService.GetOrCreateCartId();

        var cart = await _redisCacheService.GetData<List<CartItemViewModel>>(cartId) ?? new();

        var item = cart.FirstOrDefault(c => c.ProductId == id);
        if (item is not null)
        {
            item.Quantity = 1;
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

        await _redisCacheService.SetData(cartId, cart, TimeSpan.FromMinutes(30));
        return RedirectToAction(nameof(Cart));
    }

    [HttpPost]
    public async Task<IActionResult> UpdateCart(int ProductId, int Quantity)
    {


        var cartId = _redisCacheService.GetOrCreateCartId();
        var cart = await _redisCacheService.GetData<List<CartItemViewModel>>(cartId) ?? new();


        var item = cart.FirstOrDefault(c => c.ProductId == ProductId);
        if (item is not null)
        {
            if (Quantity <= 0)
                cart.Remove(item);
            else
                item.Quantity = Quantity;

            await _redisCacheService.SetData(cartId, cart, TimeSpan.FromMinutes(30));
        }

        return RedirectToAction(nameof(Cart));
    }


    [HttpPost]
    public async Task<IActionResult> RemoveCart(int ProductId)
    {
        var cartId = _redisCacheService.GetOrCreateCartId();
        var cart = await _redisCacheService.GetData<List<CartItemViewModel>>(cartId) ?? new();

        var item = cart.FirstOrDefault(c => c.ProductId == ProductId);
        if (item is not null)
            cart.Remove(item);


        await _redisCacheService.SetData(cartId, cart, TimeSpan.FromMinutes(30));


        return RedirectToAction(nameof(Index));
    }


    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var cartId = _redisCacheService.GetOrCreateCartId();
        var cart = await _redisCacheService.GetData<List<CartItemViewModel>>(cartId) ?? new();

        var viewModel = new CheckoutViewModel
        {
            CartItems = cart
        };

        return View(viewModel);
    }


    [HttpPost]
    public IActionResult ProceedToCheckout() => RedirectToAction(nameof(Index), "Checkout");





}
