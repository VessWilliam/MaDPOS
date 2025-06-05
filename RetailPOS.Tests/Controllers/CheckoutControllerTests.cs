using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetailPOS.Web.Controllers;
using RetailPOS.Web.Data;
using RetailPOS.Web.Models;
using RetailPOS.Web.Models.ViewModel;

namespace RetailPOS.Tests.Controllers
{
    public class CheckoutControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly CheckoutController _controller;

        public CheckoutControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new CheckoutController(_context);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Name = "Product 1", Price = 10, StockQuantity = 5 },
                new Product { Name = "Product 2", Price = 20, StockQuantity = 3 }
            };
            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task Process_WithValidItems_RedirectsToReceipt()
        {
            // Arrange
            var product = new Product { Name = "Test Product", Price = 10, StockQuantity = 5 };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            var model = new CheckoutViewModel
            {
                Items = new List<CheckoutItem>
                {
                    new CheckoutViewModel{ ProductId = product.Id, Quantity = 2 }
                }
            };

            // Act
            var result = await _controller.Process(model);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Receipt", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Process_WithInsufficientStock_ReturnsBadRequest()
        {
            // Arrange
            var product = new Product { Name = "Test Product", Price = 10, StockQuantity = 1 };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            var model = new CheckoutViewModel
            {
                Items = new List<CheckoutViewModel>
                {
                    new CheckoutViewModel{ ProductId = product.Id, Quantity = 2 }
                }
            };

            // Act
            var result = await _controller.Process(model);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid product or insufficient stock", badRequestResult.Value);
        }

        [Fact]
        public async Task Receipt_WithValidId_ReturnsViewWithTransaction()
        {
            // Arrange
            var transaction = new SalesTransaction
            {
                TransactionDate = DateTime.UtcNow,
                TotalAmount = 100,
                Status = "Completed"
            };
            await _context.SalesTransactions.AddAsync(transaction);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Receipt(transaction.Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<SalesTransaction>(viewResult.Model);
            Assert.Equal(transaction.Id, model.Id);
        }

        [Fact]
        public async Task Receipt_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.Receipt(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}