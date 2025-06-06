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

    public async Task<SalesTransaction?> GetReceiptTransactionWithItemsAsync(int id)
    {
        try
        {
            var result = await _saleTransactionsRepo.GetReceiptTransactionWithItemsAsync(id);

            return result is null ? null : result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error Get sale transaction receipt Error");
            return null;
        }

    }

    public async Task<int> ProcessTransactionAsync(SalesTransaction transaction, List<CartItemViewModel> cart, decimal amount)
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
}
