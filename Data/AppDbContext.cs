using Microsoft.EntityFrameworkCore;
using EShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace EShop.Data;

public class AppDbContext : IdentityDbContext<User, Role, int,
    IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>,
    IdentityRoleClaim<int>, IdentityUserToken<int>>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 設定 UserRole 的複合主鍵
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        // 設定導航屬性
        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);

        // ============ 種子資料 ============

        // 1. 種子角色
        var adminRole = new Role
        {
            Id = 1,
            Name = "Admin",
            NormalizedName = "ADMIN",
            Description = "管理員",
            CreatedAt = DateTime.UtcNow
        };

        var customerRole = new Role
        {
            Id = 2,
            Name = "Customer",
            NormalizedName = "CUSTOMER",
            Description = "客戶",
            CreatedAt = DateTime.UtcNow
        };

        modelBuilder.Entity<Role>().HasData(adminRole, customerRole);

        // 2. 種子分類
        modelBuilder.Entity<Category>().HasData(
            new Category
            {
                Id = 1,
                Name = "電子產品",
                Description = "各類電子設備",
                CreatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = 2,
                Name = "圖書",
                Description = "各類書籍",
                CreatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = 3,
                Name = "服裝",
                Description = "衣服和鞋類",
                CreatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = 4,
                Name = "家居用品",
                Description = "家庭用品",
                CreatedAt = DateTime.UtcNow
            }
        );

        // 3. 種子商品
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "iPhone 15 Pro",
                Description = "最新款蘋果手機，配備 A17 Pro 晶片",
                Price = 999.99m,
                Stock = 50,
                CategoryId = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = 2,
                Name = "MacBook Pro 16",
                Description = "高效能筆記本電腦，適合專業工作",
                Price = 2499.99m,
                Stock = 20,
                CategoryId = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = 3,
                Name = "iPad Air",
                Description = "輕薄平板電腦，適合娛樂和工作",
                Price = 599.99m,
                Stock = 35,
                CategoryId = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = 4,
                Name = "C# 完全指南",
                Description = "深入學習 C# 程式設計",
                Price = 49.99m,
                Stock = 100,
                CategoryId = 2,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = 5,
                Name = ".NET Core 實戰",
                Description = "學習如何使用 .NET Core 開發應用",
                Price = 59.99m,
                Stock = 80,
                CategoryId = 2,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = 6,
                Name = "T恤",
                Description = "舒適的棉質 T 恤",
                Price = 19.99m,
                Stock = 200,
                CategoryId = 3,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = 7,
                Name = "牛仔褲",
                Description = "經典牛仔褲",
                Price = 79.99m,
                Stock = 150,
                CategoryId = 3,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = 8,
                Name = "咖啡機",
                Description = "全自動咖啡機",
                Price = 129.99m,
                Stock = 40,
                CategoryId = 4,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = 9,
                Name = "台燈",
                Description = "LED 護眼台燈",
                Price = 39.99m,
                Stock = 60,
                CategoryId = 4,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );

        // 4. 種子使用者
        var adminUser = new User
        {
            Id = 1,
            UserName = "admin@example.com",
            NormalizedUserName = "ADMIN@EXAMPLE.COM",
            Email = "admin@example.com",
            NormalizedEmail = "ADMIN@EXAMPLE.COM",
            EmailConfirmed = true,
            PasswordHash = new PasswordHasher<User>().HashPassword(null, "Admin123!"),
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnabled = true,
            AccessFailedCount = 0,
            Name = "管理員",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var customer1User = new User
        {
            Id = 2,
            UserName = "customer1@example.com",
            NormalizedUserName = "CUSTOMER1@EXAMPLE.COM",
            Email = "customer1@example.com",
            NormalizedEmail = "CUSTOMER1@EXAMPLE.COM",
            EmailConfirmed = true,
            PasswordHash = new PasswordHasher<User>().HashPassword(null, "Customer123!"),
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnabled = true,
            AccessFailedCount = 0,
            Name = "客戶一",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var customer2User = new User
        {
            Id = 3,
            UserName = "customer2@example.com",
            NormalizedUserName = "CUSTOMER2@EXAMPLE.COM",
            Email = "customer2@example.com",
            NormalizedEmail = "CUSTOMER2@EXAMPLE.COM",
            EmailConfirmed = true,
            PasswordHash = new PasswordHasher<User>().HashPassword(null, "Customer123!"),
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnabled = true,
            AccessFailedCount = 0,
            Name = "客戶二",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        modelBuilder.Entity<User>().HasData(adminUser, customer1User, customer2User);

        // 5. 種子使用者角色
        modelBuilder.Entity<UserRole>().HasData(
            new UserRole
            {
                UserId = 1,
                RoleId = 1  // Admin
            },
            new UserRole
            {
                UserId = 2,
                RoleId = 2  // Customer
            },
            new UserRole
            {
                UserId = 3,
                RoleId = 2  // Customer
            }
        );

        // 6. 種子訂單
        modelBuilder.Entity<Order>().HasData(
            new Order
            {
                Id = 1,
                UserId = 2,
                TotalPrice = 1599.97m,
                Status = "Delivered",
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new Order
            {
                Id = 2,
                UserId = 3,
                TotalPrice = 99.98m,
                Status = "Shipped",
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            }
        );

        // 7. 種子訂單項目
        modelBuilder.Entity<OrderItem>().HasData(
            new OrderItem
            {
                Id = 1,
                OrderId = 1,
                ProductId = 1,
                Quantity = 1,
                UnitPrice = 999.99m,
                TotalPrice = 999.99m
            },
            new OrderItem
            {
                Id = 2,
                OrderId = 1,
                ProductId = 6,
                Quantity = 3,
                UnitPrice = 19.99m,
                TotalPrice = 59.97m
            },
            new OrderItem
            {
                Id = 3,
                OrderId = 2,
                ProductId = 4,
                Quantity = 2,
                UnitPrice = 49.99m,
                TotalPrice = 99.98m
            }
        );
    }
}