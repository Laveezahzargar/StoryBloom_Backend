using Microsoft.AspNetCore.Mvc;
using P9_Blog_Generator_AI_Backend.DTOs.Auth;
using P9_Blog_Generator_AI_Backend.Services.Interfaces;

namespace P9_Blog_Generator_AI_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    { 
        _authService = authService;
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);

        if (!result)
        {
            return BadRequest("Registration failed.");
        }

        return Ok("User registered successfully.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var token = await _authService.LoginAsync(dto);

        if (token == null)
        {
            return Unauthorized("Invalid email or password.");
        }

        return Ok(new
        {
            Token = token
        });
    }
}