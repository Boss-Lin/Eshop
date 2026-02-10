using EShop.Models;
using Microsoft.AspNetCore.Identity;

namespace EShop.Models;

public class Role: IdentityRole<int>
{
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // 導航屬性
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}