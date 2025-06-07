using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using RetailPOS.Web.Data;
using RetailPOS.Web.Models;
using RetailPOS.Web.Services;
using RetailPOS.Web.Services.IService;
using RetailPOS.Web;
using RetailPOS.Web.Helper;
using RetailPOS.Web.Repositories;
using RetailPOS.Web.Repositories.IRepository;
using RetailPOS.Web.API;

var builder = WebApplication.CreateBuilder(args);

//Add Mapster
MappingConfig.RegisterMapping();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)
    .EnableDetailedErrors()
    .EnableSensitiveDataLogging(false));

// Add Redis Exchnage
builder.Services.AddStackExchangeRedisCache(opt =>
{
    opt.Configuration = builder.Configuration.GetConnectionString("Redis");
    opt.InstanceName = "RetailPOS:";
});

// Add Dapper Read 
builder.Services.AddScoped<IDapperBaseService, DapperBaseService>();


// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "MaD POS API",
        Version = "v1",
        Description = "Public endpoints for product catalog and stock tracking"
    });
});

//Add Custom Services
builder.Services.AddScoped<PriceCrawlerService>();
builder.Services.AddScoped<IViewModelFactory, ViewModelFactory>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();
builder.Services.AddScoped<ISaleTransactionsService, SaleTransactionsService>();

//Repositories
builder.Services.AddTransient(typeof(IRepository<>), typeof(Respository<>));
builder.Services.AddTransient<IProductRepo, ProductRepo>();
builder.Services.AddTransient<ICategoryRepo, CategoryRepo>();
builder.Services.AddTransient<ISaleTransactionsRepo, SaleTransactionsRepo>();


//Add Seeder Database
builder.Services.AddTransient<DatabaseSeeder>();


// Add .Net Identity 
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MaD POS Ex API");
    c.RoutePrefix = "swagger";
});


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Client}/{action=Index}/{id?}");


// Seed the database
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedAync();
}

// Minimal External API 
ProductAPI.MapProductEndpoints(app);

app.Run();
