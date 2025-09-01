using Microsoft.AspNetCore.Mvc;
using MerchantApp.API.DTOs;
using MerchantApp.API.Models;
using MerchantApp.API.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

namespace MerchantApp.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    // Register a new merchant
    [HttpPost("register")]
    public IActionResult Register([FromBody] MerchantRegisterDTO dto)
    {
        // Prevent duplicate accounts
        if (_db.Merchants.Any(x => x.Email == dto.Email))
            return BadRequest("This email is already registered.");

        // Create new merchant with hashed password
        var merchant = new Merchant
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = HashPassword(dto.Password),  // SHA256 hash
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Merchants.Add(merchant);
        _db.SaveChanges();

        return Ok(new { merchant.MerchantId, merchant.Name, merchant.Email });
    }

    // Login and return JWT token
    [HttpPost("login")]
    public IActionResult Login([FromBody] MerchantLoginRequestDTO dto)
    {
        // Look up merchant by email
        var user = _db.Merchants.FirstOrDefault(x => x.Email == dto.Email);
        if (user == null) return Unauthorized();

        // Check hashed password
        if (!VerifyPassword(dto.PasswordHash, user.PasswordHash))
            return Unauthorized();

        // Get secret signing key from configuration
        var jwtKey = _config["Jwt:Key"] 
                     ?? throw new InvalidOperationException("JWT key not found in configuration");

        var key = Encoding.UTF8.GetBytes(jwtKey);
        var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddHours(1); // Token valid for 1 hour

        // Create JWT token with claims
        var token = new JwtSecurityToken(
            claims: new[]
            {
                new Claim("merchantId", user.MerchantId.ToString()),
                new Claim(ClaimTypes.Name, user.Name)
            },
            expires: expires,
            signingCredentials: creds
        );

        // Return token response DTO
        return Ok(new MerchantLoginResponseDTO
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            MerchantId = user.MerchantId,
            MerchantName = user.Name,
            ExpiresAt = expires
        });
    }

    // Utility: hash a password with SHA256
    private string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    // Utility: check if plain password matches stored hash
    private bool VerifyPassword(string password, string hash)
    {
        return HashPassword(password) == hash;
    }
}
