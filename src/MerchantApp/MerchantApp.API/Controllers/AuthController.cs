using Microsoft.AspNetCore.Mvc;
using CardManagement.Shared.DTOs;
using CardManagement.Shared.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MerchantApp.API.Controllers;

[ApiController] 
[Route("api/auth")] // Defines the base route for this controller
public class AuthController : ControllerBase
{
    private readonly IMerchantAuthService _authService; // Service for handling merchant authentication logic
    private readonly IConfiguration _config; // Configuration object to access app settings (e.g., JWT key)

    public AuthController(IMerchantAuthService authService, IConfiguration config)
    {
        _authService = authService;
        _config = config;
    }

    [HttpPost("register")] // Handles POST requests to api/auth/register
    public async Task<IActionResult> Register([FromBody] MerchantRegisterDTO dto)
    {
        if (!ModelState.IsValid) // Check if model validation failed
            return BadRequest(ModelState);

        try
        {
            // Register the merchant using the service
            var merchant = await _authService.RegisterAsync(dto.Name, dto.Email, dto.Password);

            // Return merchant info (excluding password)
            return Ok(new { merchant.MerchantId, merchant.Name, merchant.Email });
        }
        catch (InvalidOperationException ex)
        {
            // Return error if registration fails (e.g., duplicate email)
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")] // Handles POST requests to api/auth/login
    public async Task<IActionResult> Login([FromBody] MerchantLoginRequestDTO dto)
    {
        if (!ModelState.IsValid) // Check if model validation failed
            return BadRequest(ModelState);

        // Attempt login with provided email and password
        var user = await _authService.LoginAsync(dto.Email, dto.Password);
        if (user == null) return Unauthorized("Invalid credentials.");

        // Retrieve the JWT key from configuration
        var jwtKey = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT key not found");
        var key = Encoding.UTF8.GetBytes(jwtKey);

        // Define signing credentials using HMAC-SHA256
        var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

        // Token expiration time (1 hour)
        var expires = DateTime.UtcNow.AddHours(1);

        // Create JWT token with claims
        var token = new JwtSecurityToken(
            claims: new[]
            {
                new Claim("merchantId", user.MerchantId.ToString()), // Custom claim for merchant ID
                new Claim(ClaimTypes.Name, user.Name) // Standard claim for merchant name
            },
            expires: expires,
            signingCredentials: creds
        );

        // Return login response with JWT token
        return Ok(new MerchantLoginResponseDTO
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            MerchantId = user.MerchantId,
            MerchantName = user.Name,
            ExpiresAt = expires
        });
    }
}
