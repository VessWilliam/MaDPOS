using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RetailPOS.Web.Models;

namespace RetailPOS.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<SalesTransaction> SalesTransactions { get; set; } = null!;
        public DbSet<SalesTransactionItem> SalesTransactionItems { get; set; } = null!;
        public DbSet<PriceHistory> PriceHistories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Product
            builder.Entity<Product>(entity =>
            {
                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure SalesTransaction
            builder.Entity<SalesTransaction>(entity =>
            {
                entity.HasMany(s => s.Items)
                    .WithOne(i => i.SalesTransaction)
                    .HasForeignKey(i => i.SalesTransactionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure SalesTransactionItem
            builder.Entity<SalesTransactionItem>(entity =>
            {
                entity.HasOne(i => i.Product)
                    .WithMany(p => p.Items)
                    .HasForeignKey(i => i.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure PriceHistory
            builder.Entity<PriceHistory>(entity =>
            {
                entity.HasOne(h => h.Product)
                    .WithMany(p => p.PriceHistory)
                    .HasForeignKey(h => h.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}