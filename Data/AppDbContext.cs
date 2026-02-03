using Microsoft.EntityFrameworkCore;
using EShop.Models;

namespace EShop.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 設定複合主鍵
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        // ============ 種子資料 ============

        // 1. 種子角色
        modelBuilder.Entity<Role>().HasData(
            new Role 
            { 
                Id = 1, 
                Name = "Admin", 
                Description = "管理員",
                CreatedAt = DateTime.UtcNow
            },
            new Role 
            { 
                Id = 2, 
                Name = "Customer", 
                Description = "客戶",
                CreatedAt = DateTime.UtcNow
            }
        );

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
            // 電子產品
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
            // 圖書
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
            // 服裝
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
            // 家居用品
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

        // 4. 種子使用者（密碼需要加密，這裡只是示例）
        // 注意：實際應用中，密碼應該使用 BCrypt 或其他加密方法
        modelBuilder.Entity<User>().HasData(
            new User 
            { 
                Id = 1, 
                Email = "admin@example.com", 
                PasswordHash = "hashed_password_123",  // 實際應使用加密密碼
                Name = "管理員", 
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User 
            { 
                Id = 2, 
                Email = "customer1@example.com", 
                PasswordHash = "hashed_password_456",  // 實際應使用加密密碼
                Name = "客戶一", 
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User 
            { 
                Id = 3, 
                Email = "customer2@example.com", 
                PasswordHash = "hashed_password_789",  // 實際應使用加密密碼
                Name = "客戶二", 
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );

        // 5. 種子使用者角色
        modelBuilder.Entity<UserRole>().HasData(
            new UserRole 
            { 
                UserId = 1, 
                RoleId = 1  // 管理員
            },
            new UserRole 
            { 
                UserId = 2, 
                RoleId = 2  // 客戶
            },
            new UserRole 
            { 
                UserId = 3, 
                RoleId = 2  // 客戶
            }
        );

        // 6. 種子訂單（可選）
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

        // 7. 種子訂單項目（可選）
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