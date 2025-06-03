using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RetailPOS.Web.Models;
using RetailPOS.Web.Services.IService;
using RetailPOS.Web.Utility;

namespace RetailPOS.Web.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IAccountService _accountService;
    private readonly IViewModelFactory _viewModelFactory;   

    public AccountController(
        IAccountService accountService,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IViewModelFactory viewModelFactory)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _accountService = accountService;
        _viewModelFactory = viewModelFactory;
    }

    [HttpGet]
    [Authorize(Roles = UserRoleConstant.Admin)]
    public IActionResult Register() => View(_viewModelFactory.CreateRegisterViewModel());
    

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(_viewModelFactory.CreateRegisterViewModel(model));

        var (result, user) = await _accountService.RegisterUserAsync(model);

        if (result.Succeeded)
        {
            TempData["Success"] = $"You have register user {user!.FirstName + " " + user!.LastName} success.";
            return RedirectToAction("Index", "Home");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(nameof(model.Password), error.Description);
        }

        return View(_viewModelFactory.CreateRegisterViewModel(model));
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

        TempData[success ? "Success" : "Error"] = success
           ? "You have been logged out successfully."
           : "There was a problem logging you out. Please try again.";

        return RedirectToAction("Index", "Client");
    }

}
