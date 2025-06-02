using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace RetailPOS.Web.Models.ViewModel;

public class ProductViewModel
{
    public int Id { get; set; }  

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }

    [Required]
    public int? CategoryId { get; set; }

    public string? ImageUrl { get; set; }

    public string? CategoryName { get; set; }
    public IEnumerable<SelectListItem>? Categories { get; set; } 
}
