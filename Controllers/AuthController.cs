using EShop.Data;
using EShop.Models;
using EShop.Models.Response;
using EShop.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace EShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtTokenGenerator _jwtTokenGenerator;
    
    public AuthController(AppDbContext context, JwtTokenGenerator jwtTokenGenerator)
    {
        _context = context;
        _jwtTokenGenerator = jwtTokenGenerator;
    }
    
    // 使用者登入
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(ApiResponse<AuthResponse>.FailureResponse(
                "Email 和密碼不能為空",
                new List<string> { "Email 為必填", "密碼為必填" }
            ));
        }

        // 查找使用者
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
        {
            return Unauthorized(ApiResponse<AuthResponse>.FailureResponse("使用者不存在"));
        }

        // 驗證密碼
        if (!BC.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized(ApiResponse<AuthResponse>.FailureResponse("密碼不正確"));
        }

        // 獲取使用者角色
        var role = user.UserRoles.FirstOrDefault()?.Role?.Name ?? "Customer";

        // 生成令牌
        var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Email, user.Name, role);

        var response = new AuthResponse
        {
            Message = "登入成功",
            Token = token,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Role = role
            }
        };

        return Ok(ApiResponse<AuthResponse>.SuccessResponse(response, "登入成功"));
    }
    
    // 使用者註冊
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.Name))
        {
            return BadRequest(new { message = "Email、密碼和名稱不能為空" });
        }

        // 檢查使用者是否已存在
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (existingUser != null)
        {
            return BadRequest(new { message = "使用者已存在" });
        }

        // 建立新使用者
        var user = new User
        {
            Email = request.Email,
            PasswordHash = BC.HashPassword(request.Password),
            Name = request.Name,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // 指派 Customer 角色
        var customerRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Customer");
        if (customerRole != null)
        {
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = customerRole.Id
            };
            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();
        }

        return Ok(new { message = "註冊成功", userId = user.Id });
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}