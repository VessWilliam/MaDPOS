using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RetailPOS.Web.Models;

namespace RetailPOS.Web.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Create roles if they don't exist
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var roles = new[] { "Admin", "Manager", "Cashier" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Create admin user if it doesn't exist
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var adminEmail = "admin@retailpos.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Add sample categories if none exist
        if (!context.Categories.Any())
        {
            var categories = new[]
            {
                new Category { Name = "Electronics" },
                new Category { Name = "Clothing" },
                new Category { Name = "Food" },
                new Category { Name = "Books" }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }

        // Add sample products if none exist
        if (!context.Products.Any())
        {
            var electronicsCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Electronics");
            var clothingCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Clothing");

            var products = new[]
            {
                new Product
                {
                    Name = "Smartphone",
                    Description = "Latest model smartphone",
                    Price = 999.99m,
                    StockQuantity = 50,
                    ImageUrl = "https://i.pinimg.com/736x/a8/dc/5c/a8dc5cf1dad0a00983c9d1992efd20ee.jpg",
                    CategoryId = electronicsCategory?.Id
                },
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }
    }
}
