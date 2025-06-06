using RetailPOS.Web.Models.ViewModel;
using RetailPOS.Web.Models;

namespace RetailPOS.Web.Services.IService;

public interface ISaleTransactionsService
{
    Task<int> ProcessTransactionAsync(SalesTransaction transaction, List<CartItemViewModel> cart, decimal amount);
    Task<SalesTransaction?> GetTransactionWithItemsIdAsync(int id);

    Task<bool> UpdateTransactionStatusAsync(int id, string status, string paymentStatus);

    Task<List<SalesTransaction>> GetTransactionsWithItemsAsync();

    Task<SalesTransaction?> GetTransactionsWithByIdAsync(int id);
    Task<bool> CreateNewSaleTransaction(SalesTransaction model);

    Task<bool> DeleteSaleTransactionAsync(int id);

    Task<List<SalesTransaction>> GetSalesTransactionsReportAsync(DateTime? startDate, DateTime? endDate);

}