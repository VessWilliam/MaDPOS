namespace RetailPOS.Web.Models.ViewModel;

public class CartItemViewModel
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal { get; set; }
    public string? imageUrl { get; set; }
}
