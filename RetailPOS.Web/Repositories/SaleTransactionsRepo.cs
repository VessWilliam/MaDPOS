using Dapper;
using Microsoft.EntityFrameworkCore;
using RetailPOS.Web.Data;
using RetailPOS.Web.Models;
using RetailPOS.Web.Models.ViewModel;
using RetailPOS.Web.Repositories.IRepository;
using RetailPOS.Web.Services.IService;

namespace RetailPOS.Web.Repositories;

public class SaleTransactionsRepo : Respository<SalesTransaction>, ISaleTransactionsRepo
{
    private readonly IDapperBaseService _dapperBaseService;
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly ILogger<SalesTransaction> _logger;
    public SaleTransactionsRepo(
            IDapperBaseService dapperBaseService,
            IDbContextFactory<ApplicationDbContext> contextFactory,
            ILoggerFactory loggerFactory)
        : base(contextFactory)
    {
        _dapperBaseService = dapperBaseService;
        _contextFactory = contextFactory;
        _logger = loggerFactory.CreateLogger<SalesTransaction>();
    }
    public async Task<int> ProcessTransactionAsync(SalesTransaction transaction, List<CartItemViewModel> cart, decimal amount)
    {

        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var dbTransaction = await context.Database.BeginTransactionAsync();

        try
        {

            decimal total = 0;

            foreach (var item in cart)
            {
                var product = await context.Products.FindAsync(item.ProductId);
                if (product is null || product.StockQuantity < item.Quantity)
                {
                    _logger.LogError($"Invalid or out-of-stock product: {item.ProductId}");
                    return 0;
                }

                var totalPrice = product.Price * item.Quantity;

                transaction.Items.Add(new SalesTransactionItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price,
                    TotalPrice = totalPrice
                });

                total += totalPrice;

                product.StockQuantity -= item.Quantity;
            }

            if (amount < total)
            {
                _logger.LogError("Amount received is less than total.");
                return 0;
            }

            transaction.TotalAmount = total;



            await context.SalesTransactions.AddAsync(transaction);
            await context.SaveChangesAsync();

            await dbTransaction.CommitAsync();
            return transaction.Id;
        }
        catch
        {
            await dbTransaction.RollbackAsync();
            _logger.LogError("Update Sale Trans Internal Error");
            return 0;
        }
    }


    public async Task<SalesTransaction?> GetReceiptTransactionWithItemsAsync(int id)
    {
        try
        {
            var param = new DynamicParameters();
            param.Add("@Id", id);

            var query = @"
                SELECT t.*, i.*, p.*
                FROM ""SalesTransactions"" t
                INNER JOIN ""SalesTransactionItems"" i ON t.""Id"" = i.""SalesTransactionId""
                INNER JOIN ""Products"" p ON i.""ProductId"" = p.""Id""
                WHERE t.""Id"" = @Id";

            var transactionDictionary = new Dictionary<int, SalesTransaction>();

            return await _dapperBaseService.getDBConnectionAsync(async connection =>
            {
                var result = await connection.QueryAsync<SalesTransaction,
                    SalesTransactionItem,
                    Product, SalesTransaction>(
                    query,
                    (t, item, product) =>
                    {
                        if (!transactionDictionary.TryGetValue(t.Id, out var transaction))
                        {
                            transaction = t;
                            transaction.Items = new List<SalesTransactionItem>();
                            transactionDictionary.Add(t.Id, transaction);
                        }

                        item.Product = product;
                        transaction.Items.Add(item);
                        return transaction;
                    },
                    param,
                    splitOn: "Id,ProductId" 
                );

                return transactionDictionary.Values.FirstOrDefault();
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving transaction with id: {id}");
            return null;
        }
    }
}
