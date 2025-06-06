using Dapper;
using Microsoft.EntityFrameworkCore;
using RetailPOS.Web.Data;
using RetailPOS.Web.Models;
using RetailPOS.Web.Models.ViewModel;
using RetailPOS.Web.Repositories.IRepository;
using RetailPOS.Web.Services.IService;
using RetailPOS.Web.Utility;

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

    public async Task<SalesTransaction?> GetTransactionWithItemsIdAsync(int id)
    {
        try
        {
            var param = new DynamicParameters();
            param.Add("@Id", id);

            var query = @"
            SELECT
                t.""Id"" AS Id, t.""CustomerName"", t.""TransactionDate"", t.""Status"", t.""PaymentStatus"", t.""TotalAmount"",
                t.""PaymentMethod"",i.""Id"" AS Id, i.""SalesTransactionId"", i.""ProductId"", i.""Quantity"", i.""UnitPrice"", i.""TotalPrice"",
                p.""Id"" AS Id, p.""Name"", p.""Price"", p.""StockQuantity"", p.""ImageUrl""
            FROM ""SalesTransactions"" t
            INNER JOIN ""SalesTransactionItems"" i ON t.""Id"" = i.""SalesTransactionId""
            INNER JOIN ""Products"" p ON i.""ProductId"" = p.""Id""
            WHERE t.""Id"" = @Id";

            var transactionDictionary = new Dictionary<int, SalesTransaction>();

            return await _dapperBaseService.getDBConnectionAsync(async connection =>
            {
                var result = await connection.QueryAsync<SalesTransaction, SalesTransactionItem, Product, SalesTransaction>(
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

                        if (product != null && item.Quantity > 0)
                        {
                            item.UnitPrice = product.Price;
                            item.TotalPrice = product.Price * item.Quantity;
                        }

                        transaction.Items.Add(item);
                        return transaction;
                    },
                    param,
                    splitOn: "Id,Id"
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

    public async Task<List<SalesTransaction>> GetTransactionsWithItemsAsync()
    {
        try
        {
            var query = @"
            SELECT
                t.""Id"", t.""CustomerName"", t.""TransactionDate"", t.""Status"", t.""PaymentStatus"", t.""TotalAmount"",
                i.""Id"", i.""SalesTransactionId"", i.""ProductId"", i.""Quantity"", i.""UnitPrice"", i.""TotalPrice"",
                p.""Id"", p.""Name"", p.""Price"", p.""StockQuantity"", p.""ImageUrl""
            FROM ""SalesTransactions"" t
            INNER JOIN ""SalesTransactionItems"" i ON t.""Id"" = i.""SalesTransactionId""
            INNER JOIN ""Products"" p ON i.""ProductId"" = p.""Id""
            ORDER BY t.""TransactionDate"" DESC;";

            var transactionDictionary = new Dictionary<int, SalesTransaction>();

            return await _dapperBaseService.getDBConnectionAsync(async connection =>
            {
                var result = await connection.QueryAsync<SalesTransaction, SalesTransactionItem, Product, SalesTransaction>(
                    query,
                    (transaction, item, product) =>
                    {
                        if (!transactionDictionary.TryGetValue(transaction.Id, out var transactionEntry))
                        {
                            transactionEntry = transaction;
                            transactionEntry.Items = new List<SalesTransactionItem>();
                            transactionDictionary.Add(transactionEntry.Id, transactionEntry);
                        }

                        item.Product = product;
                        transactionEntry.Items.Add(item);

                        return transactionEntry;
                    },
                    splitOn: "Id,Id"
                );

                return transactionDictionary.Values.ToList();
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transactions");
            return new List<SalesTransaction>();
        }
    }

    public async Task<bool> UpdateTransactionStatusAsync(int id, string status, string paymentStatus)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            var transaction = await context.SalesTransactions
                .Include(t => t.Items)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction is null || transaction.Status is nameof(StatusEnum.COMPLETED))
                return false;

            transaction.Status = status.ToUpper();
            transaction.PaymentStatus = paymentStatus.ToUpper();

            if (transaction.Status is nameof(StatusEnum.COMPLETED))
            {
                foreach (var item in transaction.Items)
                {
                    var product = await context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        product.StockQuantity -= item.Quantity;
                    }
                }
            }

            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error update transaction with error {ex}");
            return false;
        }
    }

    public async Task<bool> CreateNewSaleTransaction(SalesTransaction model)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            foreach (var item in model.Items)
            {
                var product = await context.Products.FindAsync(item.ProductId);
                if (product == null || product.StockQuantity < item.Quantity)
                {
                    _logger.LogWarning($"Product {item.ProductId} not found or not enough stock.");
                    return false;
                }

                item.UnitPrice = product.Price;
                item.TotalPrice = product.Price * item.Quantity;
                model.TotalAmount += item.TotalPrice;
            }

            context.SalesTransactions.Add(model);
            await context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating new transaction");
            return false;
        }

    }

    public async Task<bool> DeleteSaleTransactionAsync(int id)
    {
        try
        {

            await using var context = await _contextFactory.CreateDbContextAsync();

            var sale = await context.SalesTransactions
               .Include(s => s.Items)
               .FirstOrDefaultAsync(s => s.Id == id);


            if (sale is null) return false;


            if (sale.Status is nameof(StatusEnum.COMPLETED))
                return false;



            foreach (var item in sale.Items)
            {
                var product = await context.Products.FindAsync(item.ProductId);
                if (product is not null)
                {
                    product.StockQuantity += item.Quantity;
                }
            }

            context.SalesTransactions.Remove(sale);
            await context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error delete  transaction error");
            return false;
        }


    }

    public async Task<SalesTransaction?> GetTransactionsWithByIdAsync(int id)
    {
        try
        {
            var param = new DynamicParameters();
            param.Add("@Id", id);

            const string query = @"
            SELECT s.*, i.*, p.*
            FROM ""SalesTransactions"" s
            INNER JOIN ""SalesTransactionItems"" i ON s.""Id"" = i.""SalesTransactionId""
            INNER JOIN ""Products"" p ON i.""ProductId"" = p.""Id""
            WHERE s.""Id"" = @Id;
        ";

            var transactionDictionary = new Dictionary<int, SalesTransaction>();

            return await _dapperBaseService.getDBConnectionAsync(async connection =>
            {
                var result = await connection.QueryAsync<SalesTransaction, SalesTransactionItem, Product, SalesTransaction>(
                    query,
                    (transaction, item, product) =>
                    {
                        if (!transactionDictionary.TryGetValue(transaction.Id, out var transactionEntry))
                        {
                            transactionEntry = transaction;
                            transactionEntry.Items = new List<SalesTransactionItem>();
                            transactionDictionary[transaction.Id] = transactionEntry;
                        }

                        item.Product = product;
                        transactionEntry.Items.Add(item);

                        return transactionEntry;
                    },
                    param,
                    splitOn: "Id,Id"
                );

                return transactionDictionary.Values.FirstOrDefault();
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching transaction with items and product details.");
            return null;
        }
    }
}
