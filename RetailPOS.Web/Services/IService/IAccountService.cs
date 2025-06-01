using Microsoft.AspNetCore.Identity;
using RetailPOS.Web.Models;

namespace RetailPOS.Web.Services.IService;

public interface IAccountService
{
    Task<(IdentityResult result, ApplicationUser? user)> RegisterUserAsync(RegisterViewModel model);
    Task<(IdentityResult result, ApplicationUser? user)> LoginUserAsync(LoginViewModel model); 
    Task<bool> SignOutUserAsync();
}
