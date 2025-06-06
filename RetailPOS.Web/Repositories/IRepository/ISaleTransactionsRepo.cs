using RetailPOS.Web.Models;
using RetailPOS.Web.Models.ViewModel;

namespace RetailPOS.Web.Repositories.IRepository;

public interface ISaleTransactionsRepo
{
    Task<int> ProcessTransactionAsync(SalesTransaction transaction, List<CartItemViewModel> cart, decimal amount);

    Task<SalesTransaction?> GetReceiptTransactionWithItemsAsync(int id);
}
