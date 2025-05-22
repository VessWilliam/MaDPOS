using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace RetailPOS.Web.Models
{
    public class SalesTransaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }

        public string? PaymentMethod { get; set; }
        public string? PaymentStatus { get; set; }

        public string? Notes { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public ICollection<SalesTransactionItem> Items { get; set; } = new List<SalesTransactionItem>();
    }
}