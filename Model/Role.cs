namespace EShop.Model;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // 導航屬性
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}