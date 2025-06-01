using Mapster;
using Microsoft.AspNetCore.Identity;
using RetailPOS.Web.Models;
using RetailPOS.Web.Services.IService;
using System;

namespace RetailPOS.Web.Services;

public class AccountService : IAccountService
{
    private readonly ILogger _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountService(UserManager<ApplicationUser> userManager,
        ILoggerFactory loggerFactory,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _logger = loggerFactory.CreateLogger<AccountService>();
        _signInManager = signInManager;
    }


    public async Task<(IdentityResult result, ApplicationUser? user)> RegisterUserAsync(RegisterViewModel model)
    {
        try
        {
            var user = model.Adapt<ApplicationUser>();

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded) await _signInManager.SignInAsync(user, isPersistent: false);

            return (result, user);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error Register user : {ex}");
            return (IdentityResult
                .Failed(new IdentityError
                { Description = "Unexpected error during registration." }), null);
        }
    }
    public async Task<(IdentityResult result, ApplicationUser? user)> LoginUserAsync(LoginViewModel model)
    {
        try
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (!result.Succeeded)
                return (IdentityResult.Failed(new IdentityError { Description = "Invalid login attempt." }), null);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null)
                return (IdentityResult.Failed(new IdentityError { Description = "User not found." }), null);

            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return (IdentityResult.Success, user);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error during user login: {ex}");
            return (IdentityResult.Failed(new IdentityError { Description = "Unexpected error during login." }), null);
        }
    }


    public async Task<bool> SignOutUserAsync()
    {
        try
        {
            await _signInManager.SignOutAsync();    
            return true;
        }
        catch (Exception ex)
        {

            _logger.LogError($"error sign out: {ex}");
            return false;
        }
    }
}
