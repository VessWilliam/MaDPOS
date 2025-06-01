using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RetailPOS.Web.Models;
using RetailPOS.Web.Services.IService;

namespace RetailPOS.Web.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IAccountService _accountService;

    public AccountController(
        IAccountService accountService,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _accountService = accountService;
    }

    [HttpGet]
    public IActionResult Register() => View();


    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var (result, user) = await _accountService.RegisterUserAsync(model);

        if (result.Succeeded)
        {
            TempData["Success"] = "You have register and sign in successfully.";
            return RedirectToAction("Index", "Home");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(nameof(model.Password), error.Description);
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(model);

        var (result, user) = await _accountService.LoginUserAsync(model);

        if (!result.Succeeded)
        {
            TempData["Error"] = "There was a problem logging In. Please try again.";
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        bool success = await _accountService.SignOutUserAsync();

        if (!success)
            TempData["Error"] = "There was a problem logging you out. Please try again.";
        else
            TempData["Success"] = "You have been logged out successfully.";

        return RedirectToAction("Index", "Home");
    }
}
