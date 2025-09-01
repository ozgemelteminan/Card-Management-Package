using MerchantApp.API.DTOs;

namespace MerchantApp.API.Services.Abstractions;

public interface IAuthService
{
    Task<MerchantLoginDTO> RegisterAsync(MerchantRegisterDTO dto, CancellationToken ct = default);
    Task<MerchantLoginDTO> LoginAsync(string email, string password, CancellationToken ct = default);
}
