using Xunit;
using MerchantApp.Service.Services;
using CardManagement.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace MerchantApp.Tests.Tests
{
    public class MerchantAuthServiceTests
    {
        // Helper to create MerchantAuthService with a new in-memory database
        private MerchantAuthService CreateService(out AppDbContext db)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique DB per test
                .Options;

            db = new AppDbContext(options);
            return new MerchantAuthService(db);
        }

        [Fact]
        public async Task RegisterAsync_ShouldCreateNewMerchant()
        {
            // Test that registering a new merchant works correctly
            var service = CreateService(out var db);

            var merchant = await service.RegisterAsync("Test", "test@mail.com", "123456");

            Assert.NotNull(merchant); // Merchant object should be returned
            Assert.Equal("test@mail.com", merchant.Email); // Email should match input
            Assert.NotEmpty(merchant.PasswordHash); // Password should be hashed
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnMerchant_WhenCredentialsCorrect()
        {
            // Test login succeeds with correct email and password
            var service = CreateService(out var db);

            await service.RegisterAsync("LoginUser", "login@mail.com", "pass");

            var merchant = await service.LoginAsync("login@mail.com", "pass");

            Assert.NotNull(merchant); // Should return merchant
            Assert.Equal("login@mail.com", merchant.Email); // Email matches
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnNull_WhenPasswordWrong()
        {
            // Test login fails if password is incorrect
            var service = CreateService(out var db);

            await service.RegisterAsync("WrongPassUser", "wrong@mail.com", "goodpass");

            var merchant = await service.LoginAsync("wrong@mail.com", "badpass");

            Assert.Null(merchant); // Login should fail
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnNull_WhenEmailNotExist()
        {
            // Test login fails if email does not exist in DB
            var service = CreateService(out var db);

            var merchant = await service.LoginAsync("notexist@mail.com", "pass");

            Assert.Null(merchant); // No merchant found
        }
    }
}
