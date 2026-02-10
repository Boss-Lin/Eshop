using EShop.Models;
using Microsoft.AspNetCore.Identity;

public class User : IdentityUser<int>
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // 導航屬性
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Cart> Carts { get; set; } = new List<Cart>();
}