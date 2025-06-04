namespace RetailPOS.Web.Models.ViewModel;

public class CheckoutViewModel
{
    public List<CheckoutItem> Items { get; set; } = new();
    public List<ProductViewModel> Products { get; set; } = new();
    public List<CartItemViewModel> CartItems { get; set; } = new();
}