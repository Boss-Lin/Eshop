# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Tech Stack

- **Framework:** ASP.NET Core Web API on .NET 8.0
- **ORM:** Entity Framework Core 8 with SQLite (`ecommerce.db` in dev, `/tmp/ecommerce.db` in production)
- **Auth:** ASP.NET Core Identity + JWT Bearer tokens
- **Deployment:** AWS Elastic Beanstalk (Tokyo region, `ap-northeast-1`)

## Solution Structure

Multi-project solution (`EShop.sln`) with 4 projects:

| Project | SDK | Role |
|---------|-----|------|
| `EShop.Models` | `Microsoft.NET.Sdk` | Domain entities + Request/Response DTOs |
| `EShop.Data` | `Microsoft.NET.Sdk` | EF Core DbContext + Migrations |
| `EShop.Service` | `Microsoft.NET.Sdk` | Business logic services + JWT |
| `EShop.API` | `Microsoft.NET.Sdk.Web` | Controllers + Program.cs + middleware |

**Dependency chain:** `EShop.API` → `EShop.Service` → `EShop.Data` → `EShop.Models`

**Request/response flow:** `EShop.API.Controllers` receive `EShop.Models.Request` DTOs → delegate to `EShop.Service` → services query/mutate via `EShop.Data.AppDbContext` → return `EShop.Models.Response` DTOs wrapped in `ApiResponse<T>`.

## Common Commands

```bash
# Run (development, Swagger UI at http://localhost:5075/swagger)
dotnet run --project EShop.API/EShop.API.csproj

# Run with hot reload
dotnet watch --project EShop.API/EShop.API.csproj

# Build entire solution
dotnet build EShop.sln

# Add EF migration (run from repo root)
dotnet ef migrations add <MigrationName> --project EShop.Data --startup-project EShop.API

# Apply migrations
dotnet ef database update --project EShop.Data --startup-project EShop.API

# Reset database (dev)
rm ecommerce.db ecommerce.db-shm ecommerce.db-wal && dotnet run --project EShop.API/EShop.API.csproj
```

There are no tests in this project.

## Architecture

**Authorization:** Two roles (`Admin`, `Customer`) seeded in the database. JWT tokens carry role claims. Controllers use `[Authorize(Roles = "Admin")]` or `[Authorize]`.

**Namespaces:**
- `EShop.Models` — entities (User, Product, Cart, Order...)
- `EShop.Models.Request` — input DTOs
- `EShop.Models.Response` — output DTOs (all wrapped in `ApiResponse<T>`)
- `EShop.Data` — AppDbContext
- `EShop.Service` — ProductService, CartService, OrderService, JwtTokenGenerator
- `EShop.API.Controllers` — AuthController, ProductController, CartController, OrderController

**Note for EShop.Service:** Uses `Microsoft.NET.Sdk` with `<FrameworkReference Include="Microsoft.AspNetCore.App" />`. Files that use `IConfiguration` or `ILogger<>` must explicitly add `using Microsoft.Extensions.Configuration;` / `using Microsoft.Extensions.Logging;`.

## Key Domain Entities

- `User` extends `IdentityUser<int>` (int PK, not the default string)
- `Cart` — one per user (unique index on `UserId`); `CartItem` has composite unique index on `(CartId, ProductId)`
- `Product` belongs to `Category`; no soft-delete currently

## Seed Data (`EShop.Data/AppDbContext.cs`)

Database is seeded with roles, categories, products, and test users:
- `admin@example.com` / `Admin123!` (Admin role)
- `customer1@example.com` / `Customer123!` (Customer role)
- `customer2@example.com` / `Customer123!` (Customer role)

## Configuration

- **Dev:** `EShop.API/appsettings.json` — SQLite at `ecommerce.db`, JWT expiration 60 min
- **Prod:** `EShop.API/appsettings.Production.json` — SQLite at `/tmp/ecommerce.db`, stricter logging
- JWT secret, issuer (`Eshop`), and audience (`EShopUsers`) are set in `appsettings.json`
- `ASPNETCORE_ENVIRONMENT=Production` is set via `.ebextensions/environment.config` on Elastic Beanstalk

## Deployment

Deployed to AWS Elastic Beanstalk via the `eb` CLI. The `Procfile` starts the app on port 5000:
```
web: dotnet EShop.dll --urls http://0.0.0.0:5000
```
The `.ebignore` excludes `*.db` files — the production database is created fresh on first run and persisted at `/tmp/ecommerce.db`.

Health check endpoint: `GET /health` (returns EF Core DbContext status).
