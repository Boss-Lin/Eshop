namespace EShop.DTO;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int? CategoryId { get; set; }  // ← 改成 int?
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}