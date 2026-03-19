namespace EShop.Models.Request;

public class AddToCartRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}