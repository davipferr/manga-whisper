using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MangaWhisper.Domain.Entities;
using MangaWhisper.Common.DTOs;

namespace MangaWhisper.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
            {
                return Unauthorized(new { message = "Account is locked out. Please try again later." });
            }

            return Unauthorized(new { message = "Invalid email or password." });
        }

        var token = await GenerateJwtToken(user);
        var expiresAt = DateTime.UtcNow.AddHours(24);
        var userRoles = await _userManager.GetRolesAsync(user);

        return Ok(new LoginResponseDto
        {
            Token = token,
            Email = user.Email!,
            FullName = user.FullName,
            ExpiresAt = expiresAt,
            Roles = userRoles.ToList()
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok(new { message = "Logged out successfully." });
    }

    private async Task<string> GenerateJwtToken(ApplicationUser user)
    {
        var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
                       ?? throw new InvalidOperationException("JWT_SECRET environment variable not found.");
        var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "MangaWhisperApi";
        var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "MangaWhisperApp";

        var userRoles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        if (!string.IsNullOrEmpty(user.FullName))
        {
            claims.Add(new Claim(ClaimTypes.Name, user.FullName));
        }

        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.ToUpperInvariant()));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
