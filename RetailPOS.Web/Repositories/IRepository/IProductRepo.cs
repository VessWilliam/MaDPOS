using RetailPOS.Web.Models;

namespace RetailPOS.Web.Repositories.IRepository;

public interface IProductRepo
{
  Task<Product?> CreateProductAsync(Product model);

  Task<Product?> UpdateProductAsync(Product model);

  Task<bool> DeleteProductAsync(int id);
}
