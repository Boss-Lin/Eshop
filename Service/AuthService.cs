using EShop.Data;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace EShop.Service;

public class AuthService
{
    // private readonly AppDbContext _context;
    // private readonly JwtTokenGenerator _tokenGenerator;
    //
    // public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    // {
    //     var existingUser = await _context.Users
    //         .FirstOrDefaultAsync(u => u.Email == request.Email);
    //     
    //     if (existingUser != null)
    //         throw new InvalidOperationException("Email already registered");
    //
    //     var user = new User
    //     {
    //         Email = request.Email,
    //         Name = request.Name,
    //         PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
    //     };
    //
    //     _context.Users.Add(user);
    //     await _context.SaveChangesAsync();
    //
    //     return new AuthResponse
    //     {
    //         UserId = user.Id,
    //         Email = user.Email,
    //         Token = _tokenGenerator.GenerateToken(user)
    //     };
    // }
    //
    // public async Task<AuthResponse> LoginAsync(LoginRequest request)
    // {
    //     var user = await _context.Users
    //         .Include(u => u.UserRoles)
    //         .FirstOrDefaultAsync(u => u.Email == request.Email);
    //
    //     if (user == null || 
    //         !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
    //         throw new UnauthorizedAccessException("Invalid credentials");
    //
    //     return new AuthResponse
    //     {
    //         UserId = user.Id,
    //         Email = user.Email,
    //         Token = _tokenGenerator.GenerateToken(user)
    //     };
    // }
}