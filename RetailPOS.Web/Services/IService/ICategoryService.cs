using RetailPOS.Web.Models;

namespace RetailPOS.Web.Services.IService;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetCategoryAsync();
}
