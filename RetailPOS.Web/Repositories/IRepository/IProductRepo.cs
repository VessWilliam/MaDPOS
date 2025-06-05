using RetailPOS.Web.Models;

namespace RetailPOS.Web.Repositories.IRepository;

public interface IProductRepo
{
  Task<Product?> CreateProductAsync(Product model);

  Task<Product?> UpdateProductAsync(Product model);

  Task<bool> DeleteProductAsync(int id);

  Task<Product?> GetProductViewModelWithIdAsync(int? id);

  Task<IEnumerable<Product>> GetProductViewModelListsAsync();

  Task<IEnumerable<Product>> GetCheckOutProductListAsync();

}
