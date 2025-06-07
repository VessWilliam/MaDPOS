using RetailPOS.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace RetailPOS.Web.API;

public static class ProductAPI
{
    public static void MapProductEndpoints(IEndpointRouteBuilder app)
    {
        // GET /api/products
        app.MapGet("/products", async ([FromServices] IProductService productService) =>
        {
            var products = await productService.GetCheckOutProductListAsync();


            if (products is null) return Results.NotFound(products);


            var result = products.Select(p => new
            {
                p.Id,
                p.Name,
                p.Price,
                p.StockQuantity
            }).OrderBy(i => i.Name);



            return Results.Ok(products);
        });

        // GET /api/products/{id}
        app.MapGet("/products/{id:int}", async (int id, [FromServices] IProductService productService) =>
        {
            var product = await productService.GetProductViewModelWithIdAsync(id);

            if (product is null) return Results.NotFound();

            return Results.Ok(product);
        });
    }
}