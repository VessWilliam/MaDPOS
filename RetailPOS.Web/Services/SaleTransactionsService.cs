using RetailPOS.Web.Models;
using RetailPOS.Web.Models.ViewModel;
using RetailPOS.Web.Repositories.IRepository;
using RetailPOS.Web.Services.IService;

namespace RetailPOS.Web.Services;

public class SaleTransactionsService : ISaleTransactionsService
{
    private readonly ILogger _logger;
    private readonly ISaleTransactionsRepo _saleTransactionsRepo;
    public SaleTransactionsService(ILoggerFactory loggerFactory,
        ISaleTransactionsRepo saleTransactionsRepo)
    {
        _logger = loggerFactory.CreateLogger<SaleTransactionsService>();
        _saleTransactionsRepo = saleTransactionsRepo;
    }

    public async Task<bool> CreateNewSaleTransaction(SalesTransaction model)
    {
        try
        {
            var result = await _saleTransactionsRepo.CreateNewSaleTransaction(model);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error Get sale transaction List Error");
            return false;
        }
    }

    public async Task<List<SalesTransaction>> GetTransactionsWithItemsAsync()
    {
        try
        {
            var result = await _saleTransactionsRepo.GetTransactionsWithItemsAsync();
            return result is null ? new List<SalesTransaction>() : result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error Get sale transaction List Error");
            return new List<SalesTransaction>();
        }
    }

    public async Task<SalesTransaction?> GetTransactionWithItemsIdAsync(int id)
    {
        try
        {
            var result = await _saleTransactionsRepo.GetTransactionWithItemsIdAsync(id);

            return result is null ? null : result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error Get sale transaction  Error");
            return null;
        }

    }

    public async Task<int> ProcessTransactionAsync(SalesTransaction transaction,
        List<CartItemViewModel> cart, decimal amount)
    {
        try
        {
            var result = await _saleTransactionsRepo.ProcessTransactionAsync(transaction, cart, amount);
            return result is 0 ? 0 : result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error update sale transaction Error");
            return 0;
        }
    }

    public async Task<bool> UpdateTransactionStatusAsync(int id, string status, string paymentStatus)
    {
        try
        {
            var result = await _saleTransactionsRepo.UpdateTransactionStatusAsync(id, status, paymentStatus);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error update sale transaction status Error");
            return false;
        }
    }
}
