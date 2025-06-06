using RetailPOS.Web.Models.ViewModel;
using RetailPOS.Web.Models;

namespace RetailPOS.Web.Services.IService;

public interface ISaleTransactionsService
{
    Task<int> ProcessTransactionAsync(SalesTransaction transaction, List<CartItemViewModel> cart, decimal amount);
    Task<SalesTransaction?> GetReceiptTransactionWithItemsAsync(int id);

    Task<bool> UpdateTransactionStatusAsync(int id, string status, string paymentStatus);

}
