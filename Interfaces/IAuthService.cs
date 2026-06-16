using P9_Blog_Generator_AI_Backend.DTOs.Auth;

namespace P9_Blog_Generator_AI_Backend.Services.Interfaces;

public interface IAuthService
{
    Task<bool> RegisterAsync(RegisterDto dto);

    Task<string?> LoginAsync(LoginDto dto);

}