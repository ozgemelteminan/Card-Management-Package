using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CardManagement.Data;
using CardManagement.Shared.Models;
using CardManagement.Shared.Interfaces;
using MerchantApp.Service.Utils;

namespace MerchantApp.Service.Services
{
    public class MerchantAuthService : IMerchantAuthService
    {
        private readonly AppDbContext _db; // Database context

        public MerchantAuthService(AppDbContext db)
        {
            _db = db;
        }


        // Register a new merchant
        public async Task<Merchant> RegisterAsync(string name, string email, string password)
        {
            // Check if email is already registered
            if (await _db.Merchants.AnyAsync(x => x.Email == email))
                throw new InvalidOperationException("This email is already registered.");

            // Create a new merchant
            var merchant = new Merchant
            {
                Name = name,
                Email = email,
                PasswordHash = PasswordHasher.HashSha256(password), // Hash password
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Merchants.Add(merchant); // Add to DB
            await _db.SaveChangesAsync(); // Save changes
            return merchant;
        }

        // Login a merchant
        public async Task<Merchant?> LoginAsync(string email, string password)
        {
            // Find merchant by email
            var merchant = await _db.Merchants.FirstOrDefaultAsync(m => m.Email == email);
            if (merchant == null) return null;

            // Verify password
            var ok = PasswordHasher.VerifySha256(password, merchant.PasswordHash);
            return ok ? merchant : null;
        }
    }
}
