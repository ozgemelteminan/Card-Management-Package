namespace CardManagement.Shared.Models;

public class TransactionProductDetail
{
    public int TransactionProductDetailId { get; set; }
    public int TransactionId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public Transaction Transaction { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
