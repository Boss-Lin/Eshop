# EShop 系統流程圖

```mermaid
flowchart TD
    Client([Client / Browser])

    Client -->|HTTP Request| nginx

    subgraph AWS Elastic Beanstalk
        nginx[nginx Reverse Proxy\nport 80]

        nginx -->|port 5000| MW

        subgraph ASP.NET Core Pipeline
            MW[Middleware\nCORS / HTTPS / Routing]
            MW --> Auth[Authentication\nJWT Bearer 驗證]
            Auth --> Authz[Authorization\n角色檢查 Admin / Customer]
        end

        Authz -->|通過| Controllers

        subgraph Controllers
            AC[AuthController\n/api/auth]
            PC[ProductController\n/api/product]
            CC[CartController\n/api/cart]
        end

        Controllers --> Services

        subgraph Services
            AS[AuthService\n註冊 / 登入]
            JW[JwtTokenGenerator\n產生 JWT Token]
            PS[ProductService\n商品 CRUD]
            CS[CartService\n購物車操作]
        end

        AS --> JW
        Services --> DB

        subgraph Data Layer
            DB[AppDbContext\nEntity Framework Core]
        end

        DB --> SQLite[(SQLite\n/tmp/ecommerce.db)]
    end

    subgraph 特殊端點
        Health[GET /health\nDB 健康檢查]
        Swagger[GET /swagger\nAPI 文件]
    end

    nginx --> Health
    nginx --> Swagger

    subgraph 回應流程
        R1[200 OK + JWT Token] -->|登入成功| Client
        R2[200 OK + Data] -->|一般請求| Client
        R3[401 Unauthorized] -->|未登入| Client
        R4[403 Forbidden] -->|權限不足| Client
    end
```

## 說明

| 層次 | 元件 | 職責 |
|------|------|------|
| 入口 | nginx | EB 自動配置，轉發到 port 5000 |
| Middleware | JWT Auth | 驗證 Token，附加使用者身份 |
| Controller | Auth / Product / Cart | 接收請求、驗證輸入、回傳結果 |
| Service | AuthService 等 | 商業邏輯，不直接接觸 HTTP |
| Data | AppDbContext | EF Core 操作 SQLite |

## 認證流程

1. 登入 → `AuthService` 驗證密碼 → `JwtTokenGenerator` 產生 Token
2. 後續請求帶上 `Authorization: Bearer <token>`
3. Middleware 驗證 Token → 取出角色（Admin / Customer）
4. `[Authorize(Roles = "Admin")]` 決定能否存取
```
