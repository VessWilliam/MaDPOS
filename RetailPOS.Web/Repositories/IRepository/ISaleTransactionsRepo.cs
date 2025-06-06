using Microsoft.EntityFrameworkCore.Internal;
using RetailPOS.Web.Models;
using RetailPOS.Web.Models.ViewModel;
using RetailPOS.Web.Utility;

namespace RetailPOS.Web.Repositories.IRepository;

public interface ISaleTransactionsRepo
{
    Task<int> ProcessTransactionAsync(SalesTransaction transaction, List<CartItemViewModel> cart, decimal amount);

    Task<SalesTransaction?> GetTransactionWithItemsIdAsync(int id);

    Task<bool> UpdateTransactionStatusAsync(int id, string status, string paymentStatus);
    
}
