using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using P9_Blog_Generator_AI_Backend.Data;
using P9_Blog_Generator_AI_Backend.DTOs.Auth;
using P9_Blog_Generator_AI_Backend.Models;
using P9_Blog_Generator_AI_Backend.Services.Interfaces;
//using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace P9_Blog_Generator_AI_Backend.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<bool> RegisterAsync(RegisterDto dto)
    {
        //Log.Information("User registration started for Email: {Email}", dto.Email);
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (existingUser != null)
        {
    //        Log.Warning("Registration failed. Email already exists: {Email}",
    //dto.Email);
            return false;
        }

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

    //    Log.Information("User registered successfully. UserId: {UserId}, Email: {Email}",
    //user.UserId, user.Email);

        return true;
    }

    public async Task<string?> LoginAsync(LoginDto dto)
    {
        //Log.Information("Login attempt for Email: {Email}", dto.Email);

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null)
        {
    //        Log.Warning("Login failed. Invalid credentials for Email: {Email}",
    //dto.Email);
            return null;
        }

        bool isPasswordValid =
            BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

        if (!isPasswordValid)
        {
    //        Log.Warning("Login failed. Invalid credentials for Email: {Email}",
    //dto.Email);
            return null;
        }

    //    Log.Information("User logged in successfully. UserId: {UserId}",
    //user.UserId);

        return GenerateJwtToken(user);
    }


    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new Claim(ClaimTypes.Name, user.Name),
        new Claim(ClaimTypes.Email, user.Email)
    };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"]!));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}