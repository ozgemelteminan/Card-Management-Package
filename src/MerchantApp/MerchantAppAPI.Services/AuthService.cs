using MerchantApp.API.Data;
using MerchantApp.API.DTOs;
using MerchantApp.API.Models;
using MerchantApp.API.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MerchantApp.API.Services;

public record JwtOptions
{
    public string Key { get; init; } = "";
    public string Issuer { get; init; } = "";
    public string Audience { get; init; } = "";
    public int ExpiresMinutes { get; init; } = 60;
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly JwtOptions _jwt;

    public AuthService(AppDbContext db, IOptions<JwtOptions> jwtOptions)
    {
        _db = db;
        _jwt = jwtOptions.Value;
    }

    public async Task<MerchantLoginDTO> RegisterAsync(MerchantRegisterDTO dto, CancellationToken ct = default)
    {
        if (await _db.Merchants.AnyAsync(m => m.Email == dto.Email, ct))
            throw new InvalidOperationException("Email already registered.");

        var merchant = new Merchant
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = HashPassword(dto.Password),
            CreatedAt = DateTime.UtcNow
        };

        await _db.Merchants.AddAsync(merchant, ct);
        await _db.SaveChangesAsync(ct);

        return await CreateLoginResponseAsync(merchant, ct);
    }

    public async Task<MerchantLoginDTO> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        var merchant = await _db.Merchants.FirstOrDefaultAsync(m => m.Email == email, ct);
        if (merchant is null || !VerifyPassword(password, merchant.PasswordHash))
            throw new InvalidOperationException("Invalid credentials.");

        return await CreateLoginResponseAsync(merchant, ct);
    }

    private async Task<MerchantLoginDTO> CreateLoginResponseAsync(Merchant merchant, CancellationToken ct)
    {
        var expires = DateTime.UtcNow.AddMinutes(_jwt.ExpiresMinutes);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, merchant.Email),
            new Claim("merchantId", merchant.MerchantId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return new MerchantLoginDTO
        {
            Token = jwt,
            MerchantId = merchant.MerchantId,
            MerchantName = merchant.Name,
            ExpiresAt = expires
        };
    }

    private static string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    private static bool VerifyPassword(string password, string hash) => HashPassword(password) == hash;
}
