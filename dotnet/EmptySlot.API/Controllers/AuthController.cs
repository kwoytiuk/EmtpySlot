using Microsoft.AspNetCore.Mvc;
using EmptySlot.API.Services;

namespace EmptySlot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var (success, token, profile, error) = await _authService.RegisterAsync(
            request.Email, request.Password, request.FullName);

        if (!success)
        {
            return BadRequest(new { error });
        }

        return Ok(new { token, profile });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var (success, token, profile, error) = await _authService.LoginAsync(
            request.Email, request.Password);

        if (!success)
        {
            return Unauthorized(new { error });
        }

        return Ok(new { token, profile });
    }
}

public record RegisterRequest(string Email, string Password, string FullName);
public record LoginRequest(string Email, string Password);
