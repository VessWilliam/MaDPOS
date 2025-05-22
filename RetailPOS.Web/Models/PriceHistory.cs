using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailPOS.Web.Models
{
    public class PriceHistory
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal OldPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal NewPrice { get; set; }

        [Required]
        public DateTime ChangeDate { get; set; } = DateTime.UtcNow;

        public string? ChangedBy { get; set; }
        public string? Reason { get; set; }
    }
}