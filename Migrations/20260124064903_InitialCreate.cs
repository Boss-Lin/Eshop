using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EShop.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    Stock = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Carts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    RoleId = table.Column<int>(type: "INTEGER", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CartId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_Carts_CartId",
                        column: x => x.CartId,
                        principalTable: "Carts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 24, 6, 49, 3, 224, DateTimeKind.Utc).AddTicks(3280), "各類電子設備", "電子產品" },
                    { 2, new DateTime(2026, 1, 24, 6, 49, 3, 224, DateTimeKind.Utc).AddTicks(3280), "各類書籍", "圖書" },
                    { 3, new DateTime(2026, 1, 24, 6, 49, 3, 224, DateTimeKind.Utc).AddTicks(3280), "衣服和鞋類", "服裝" },
                    { 4, new DateTime(2026, 1, 24, 6, 49, 3, 224, DateTimeKind.Utc).AddTicks(3290), "家庭用品", "家居用品" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 24, 6, 49, 3, 224, DateTimeKind.Utc).AddTicks(3180), "管理員", "Admin" },
                    { 2, new DateTime(2026, 1, 24, 6, 49, 3, 224, DateTimeKind.Utc).AddTicks(3190), "客戶", "Customer" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "IsActive", "Name", "PasswordHash", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 24, 6, 49, 3, 224, DateTimeKind.Utc).AddTicks(3410), "admin@example.com", true, "管理員", "hashed_password_123", new DateTime(2026, 1, 24, 6, 49, 3, 224, DateTimeKind.Utc).AddTicks(3410) },
                    { 2, new DateTime(2026, 1, 24, 6, 49, 3, 224, DateTimeKind.Utc).AddTicks(3420), "customer1@example.com", true, "客戶一", "hashed_password_456", new DateTime(2026, 1, 24, 6, 49, 3, 224, DateTimeKind.Utc).AddTicks(3420) },
                    { 3, new DateTime(2026, 1, 24, 6, 49, 3, 224, DateTimeKind.Utc).AddTicks(3420), "customer2@example.com", true, "客戶二", "hashed_password_789", new DateTime(2026, 1, 24, 6, 49, 3, 224, DateTimeKind.Utc).AddTicks(3420) }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "CreatedAt", "Status", "TotalPrice", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 14, 6, 49, 3, 224, DateTimeKind.Utc).AddTicks(3450), "Delivered", 1599.97m, new DateTime(2026, 1, 19, 6, 49, 3, 224, DateTimeKind.Utc).AddTicks(3460), 2 },
                    { 2, new DateTime(2026, 1, 21, 6, 49, 3, 224, DateTimeKind.Utc).AddTicks(3460), "Shipped", 99.98m, new DateTime(2026, 1, 23, 6, 49, 3, 224, DateTimeKind.Utc).AddTicks(3460), 3 }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "CreatedBy", "Description", "Name", "Price", "Stock", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 1, 24, 6, 49, 3, 224, DateTimeKind.Utc).AddTicks(3310), null, "最新款蘋果手機，配備 A17 Pro 晶片", "iPhone 15 Pro", 999.99m, 50, new DateTime(2026, 1, 24, 6, 49, 3, 224, DateTimeKind.Utc).AddTicks(3310) },
                    { 2, 1, new DateTime(2026, 1, 24, 6, 49, 3, 224, DateTimeKind.Utc).AddTicks(3310), null, "高效能筆記本電腦，適合專業工作", "MacBook Pro 16", 2499.99m, 20, new DateTime(2026, 1, 24, 6, 49, 3, 224, DateTimeKind.Utc).AddTicks(3310) },
                    { 3, 1, new DateTime(2026, 1, 24, 6, 49, 3, 224, DateTimeKind.Utc).AddTicks(3310), null, "輕薄平板電腦，適合娛樂和工作", "iPad Air", 599.99m, 35, new DateTime(2026, 1, 24, 6, 49, 3, 224, DateTimeKind.Utc).AddTicks(3310) },
                    { 4, 2, new DateTime(2026, 1, 24, 6, 49, 3, 224, DateTimeKind.Utc).AddTicks(3320), null, "深入學習 C# 程式設計", "C# 完全指南", 49.99m, 100, new DateTime(2026, 1, 24, 6, 49, 3, 224, DateTimeKind.Utc).AddTicks(3320) },
                    { 5, 2, new DateTime(2026, 1, 24, 6, 49, 3, 224, DateTimeKind.Utc).AddTicks(3320), null, "學習如何使用 .NET Core 開發應用", ".NET Core 實戰", 59.99m, 80, new DateTime(2026, 1, 24, 6, 49, 3, 224, DateTimeKind.Utc).AddTicks(3320) },
                    { 6, 3, new DateTime(2026, 1, 24, 6, 49, 3, 224, DateTimeKind.Utc).AddTicks(3320), null, "舒適的棉質 T 恤", "T恤", 19.99m, 200, new DateTime(2026, 1, 24, 6, 49, 3, 224, DateTimeKind.Utc).AddTicks(3320) },
                    { 7, 3, new DateTime(2026, 1, 24, 6, 49, 3, 224, DateTimeKind.Utc).AddTicks(3350), null, "經典牛仔褲", "牛仔褲", 79.99m, 150, new DateTime(2026, 1, 24, 6, 49, 3, 224, DateTimeKind.Utc).AddTicks(3350) },
                    { 8, 4, new DateTime(2026, 1, 24, 6, 49, 3, 224, DateTimeKind.Utc).AddTicks(3350), null, "全自動咖啡機", "咖啡機", 129.99m, 40, new DateTime(2026, 1, 24, 6, 49, 3, 224, DateTimeKind.Utc).AddTicks(3350) },
                    { 9, 4, new DateTime(2026, 1, 24, 6, 49, 3, 224, DateTimeKind.Utc).AddTicks(3350), null, "LED 護眼台燈", "台燈", 39.99m, 60, new DateTime(2026, 1, 24, 6, 49, 3, 224, DateTimeKind.Utc).AddTicks(3350) }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId", "Id" },
                values: new object[,]
                {
                    { 1, 1, 0 },
                    { 2, 2, 0 },
                    { 2, 3, 0 }
                });

            migrationBuilder.InsertData(
                table: "OrderItems",
                columns: new[] { "Id", "OrderId", "ProductId", "Quantity", "TotalPrice", "UnitPrice" },
                values: new object[,]
                {
                    { 1, 1, 1, 1, 999.99m, 999.99m },
                    { 2, 1, 6, 3, 59.97m, 19.99m },
                    { 3, 2, 4, 2, 99.98m, 49.99m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId",
                table: "CartItems",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductId",
                table: "CartItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_UserId",
                table: "Carts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
