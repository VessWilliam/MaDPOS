namespace RetailPOS.Web.Models;

public class CheckoutItem
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }

    public string? ProductName { get; set; }
    public decimal? UnitPrice { get; set; }
}