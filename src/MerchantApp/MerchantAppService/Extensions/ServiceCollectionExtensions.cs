using Microsoft.Extensions.DependencyInjection;
using CardManagement.Shared.Interfaces;
using MerchantApp.Service.Services; 

namespace MerchantApp.Service.Extensions
{
    public static class ServiceCollectionExtensions
    {
        // Registers all MerchantApp services to the DI container
        public static IServiceCollection AddMerchantAppServices(this IServiceCollection services)
        {
            // Register authentication service for merchants
            services.AddScoped<IMerchantAuthService, MerchantAuthService>(); 

            // Register cart service for shopping cart operations
            services.AddScoped<ICartService, CartService>(); 

            // Register transaction service for handling payments and transactions
            services.AddScoped<ITransactionService, TransactionService>(); 

            return services; // Return the updated service collection
        }
    }
}
