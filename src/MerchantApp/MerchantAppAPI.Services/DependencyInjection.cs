using MerchantApp.API.Services.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MerchantApp.API.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddMerchantAppServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IQrService, QrService>();
        services.AddScoped<IAuthService, AuthService>();

        services.Configure<JwtOptions>(config.GetSection("Jwt"));

        return services;
    }
}
