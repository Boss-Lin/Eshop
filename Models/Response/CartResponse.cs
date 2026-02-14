namespace EShop.Models.Response;

public class CartResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public List<CartItemResponse> Items { get; set; }
    public decimal TotalAmount { get; set; }
    public int TotalItems { get; set; }
    public DateTime UpdatedAt { get; set; }
}