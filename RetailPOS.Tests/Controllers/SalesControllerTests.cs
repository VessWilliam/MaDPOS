using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetailPOS.Web.Controllers;
using RetailPOS.Web.Data;
using RetailPOS.Web.Models;
using System;
using System.Threading.Tasks;
using Xunit;

namespace RetailPOS.Tests.Controllers
{
    public class SalesControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly SalesController _controller;

        public SalesControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new SalesController(_context);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfSales()
        {
            // Arrange
            var sales = new List<SalesTransaction>
            {
                new SalesTransaction { TransactionDate = DateTime.UtcNow, TotalAmount = 100 },
                new SalesTransaction { TransactionDate = DateTime.UtcNow, TotalAmount = 200 }
            };
            await _context.SalesTransactions.AddRangeAsync(sales);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<SalesTransaction>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task Create_WithValidData_RedirectsToIndex()
        {
            // Arrange
            var sale = new SalesTransaction
            {
                TransactionDate = DateTime.UtcNow,
                TotalAmount = 100,
                Status = "Completed"
            };

            // Act
            var result = await _controller.Create(sale);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Create_WithInvalidData_ReturnsViewWithModel()
        {
            // Arrange
            var sale = new SalesTransaction(); // Invalid sale with no data
            _controller.ModelState.AddModelError("TotalAmount", "Required");

            // Act
            var result = await _controller.Create(sale);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(sale, viewResult.Model);
        }
    }
}