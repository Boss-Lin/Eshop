using EShop.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EShop.Models;
using EShop.Service;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 配置 Serilog 日誌
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/eshop-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("啟動 EShop 應用程式");

    // 讀取 JWT 配置
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey 未配置");
    var issuer = jwtSettings["Issuer"] ?? "Eshop";
    var audience = jwtSettings["Audience"] ?? "EShopUsers";
    var expiryInMinutes = jwtSettings.GetValue<int>("ExpiryInMinutes", 60);

    // 配置資料庫
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=ecommerce.db";

    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseSqlite(connectionString);
        if (builder.Environment.IsDevelopment())
        {
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        }
    });

    // 配置 Identity（移除重複配置，統一使用 User 和 Role）
    builder.Services.AddIdentity<User, Role>(options =>
    {
        // 密碼要求
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;

        // 使用者要求
        options.User.RequireUniqueEmail = true;

        // 登入設定
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

    // 配置 JWT 驗證
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "Bearer";
        options.DefaultChallengeScheme = "Bearer";
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero // 移除預設的 5 分鐘寬限時間
        };

        // 添加事件處理（用於除錯）
        options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Log.Warning("JWT 驗證失敗: {Error}", context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Log.Information("JWT 驗證成功: {User}", context.Principal?.Identity?.Name);
                return Task.CompletedTask;
            }
        };
    });

    // 配置授權策略
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        options.AddPolicy("UserOrAdmin", policy => policy.RequireRole("User", "Admin"));
    });

    // 配置 CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });

        options.AddPolicy("Production", builder =>
        {
            builder.WithOrigins(
                    "https://yourdomain.com",
                    "https://www.yourdomain.com"
                )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });

    // 註冊應用程式服務
    builder.Services.AddScoped<JwtTokenGenerator>();
    builder.Services.AddScoped<ProductService>();
    // 可以在這裡添加更多服務
    // builder.Services.AddScoped<OrderService>();
    builder.Services.AddScoped<CartService>();

    // 配置 Controllers 和 JSON 選項
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler =
                System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.DefaultIgnoreCondition =
                System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        });

    // 配置 Swagger/OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "EShop API",
            Description = "電子商務系統 API 文件",
            Contact = new OpenApiContact
            {
                Name = "開發團隊",
                Email = "dev@eshop.com"
            }
        });

        // 配置 JWT 安全定義
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "請輸入 JWT Token，格式：Bearer {your token}"
        });

        // 配置安全要求
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });

        // 添加 XML 註釋（如果有的話）
        // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        // if (File.Exists(xmlPath))
        // {
        //     options.IncludeXmlComments(xmlPath);
        // }
    });

    // 配置 Health Checks
    builder.Services.AddHealthChecks()
        .AddDbContextCheck<AppDbContext>();

    var app = builder.Build();

    // 初始化資料庫
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<AppDbContext>();
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<Role>>();

            // 確保資料庫已建立
            context.Database.EnsureCreated();

            // 初始化角色和管理員帳號（可選）
            // await SeedDataAsync(context, userManager, roleManager);

            Log.Information("資料庫初始化完成");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "資料庫初始化時發生錯誤");
        }
    }

    // 配置 HTTP 請求管線
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "EShop API v1");
            options.RoutePrefix = "swagger"; // 設定 Swagger UI 為根路徑
        });

        app.UseCors("AllowAll");
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/error");
        app.UseHsts();
        app.UseCors("Production");
    }

    // 中介軟體順序很重要
    app.UseHttpsRedirection();
    app.UseStaticFiles(); // 如果有靜態檔案

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    // 健康檢查端點
    app.MapHealthChecks("/health");

    app.MapControllers();

    Log.Information("EShop 應用程式啟動成功");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "應用程式啟動失敗");
}
finally
{
    Log.CloseAndFlush();
}

// 資料庫種子資料方法
// static async Task SeedDataAsync(AppDbContext context, UserManager<User> userManager, RoleManager<Role> roleManager)
// {
//     // 建立角色
//     string[] roleNames = { "Admin", "User", "Manager" };
//     foreach (var roleName in roleNames)
//     {
//         if (!await roleManager.RoleExistsAsync(roleName))
//         {
//             await roleManager.CreateAsync(new Role { Name = roleName });
//             Log.Information("建立角色: {RoleName}", roleName);
//         }
//     }
//
//     // 建立預設管理員帳號
//     var adminEmail = "admin@eshop.com";
//     var adminUser = await userManager.FindByEmailAsync(adminEmail);
//
//     if (adminUser == null)
//     {
//         adminUser = new User
//         {
//             UserName = "admin",
//             Email = adminEmail,
//             EmailConfirmed = true
//         };
//
//         var result = await userManager.CreateAsync(adminUser, "Admin@123");
//
//         if (result.Succeeded)
//         {
//             await userManager.AddToRoleAsync(adminUser, "Admin");
//             Log.Information("建立預設管理員帳號: {Email}", adminEmail);
//         }
//         else
//         {
//             Log.Error("建立管理員帳號失敗: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
//         }
//     }
// }