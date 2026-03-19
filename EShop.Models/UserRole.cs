using Microsoft.AspNetCore.Identity;

namespace EShop.Models;

public class UserRole : IdentityUserRole<int>
{
    // 導航屬性
    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}