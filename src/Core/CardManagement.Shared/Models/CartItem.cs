namespace CardManagement.Shared.Models;

public class CartItem
{
    public int CartItemId { get; set; }
    public int MerchantId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
