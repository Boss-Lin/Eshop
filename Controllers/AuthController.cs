using EShop.Data;
using EShop.Models.Request;
using EShop.Models.Response;
using EShop.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtTokenGenerator _jwtTokenGenerator;
    private readonly UserManager<User> _userManager;

    public AuthController(UserManager<User> userManager, JwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    // 使用者註冊
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<AuthResponse>.FailureResponse("輸入資料無效"));

        var user = new User
        {
            UserName = registerRequest.Email,
            Email = registerRequest.Email,
            Name = registerRequest.Name,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var identityResult = await _userManager.CreateAsync(user, registerRequest.Password);

        if (!identityResult.Succeeded)
        {
            var errors = identityResult.Errors.Select(e => e.Description).ToList();
            return BadRequest(ApiResponse<AuthResponse>.FailureResponse("註冊失敗", errors));
        }

        // 新增為 Customer 角色
        await _userManager.AddToRoleAsync(user, "Customer");

        return Ok(ApiResponse<AuthResponse>.SuccessResponse(null, "註冊成功"));
    }

    // 使用者登入
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request){
        // 1. 雖然 [ApiController] 會自動檢查，但手動保留這塊可增加自定義彈性
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<AuthResponse>.FailureResponse("輸入資料無效"));

        // 2. 尋找使用者
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            return Unauthorized(ApiResponse<AuthResponse>.FailureResponse("電子郵件或密碼不正確"));

        // 4. 取得角色 (考慮到一名用戶可能有多個角色，若只取第一個，建議增加預設值)
        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Customer";

        // 5. 產生 Token
        var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Email ?? "", user.Name ?? "User", role);

        var response = new AuthResponse
        {
            Message = "登入成功",
            Token = token,
            User = new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Role = role
            }
        };
        return Ok(ApiResponse<AuthResponse>.SuccessResponse(response, "登入成功"));
    }
}