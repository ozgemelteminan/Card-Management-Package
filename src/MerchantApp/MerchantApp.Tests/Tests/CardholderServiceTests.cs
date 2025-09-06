using System;
using System.Linq;
using System.Threading.Tasks;
using CardManagement.Data;
using CardManagement.Shared.DTOs;
using CardManagement.Shared.Models;
using MerchantApp.Service.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MerchantApp.Tests.Tests
{
    public class CardholderServiceTests
    {
        // Helper method to create an in-memory database context
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique DB per test
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task CreateCardholderAsync_ShouldCreateCardholder()
        {
            // Arrange: create DB and service instance
            var db = GetDbContext();
            var service = new CardholderService(db);

            var dto = new CardholderCreateDTO
            {
                FullName = "Test User",
                Email = "test@example.com",
                Password = "Password123!"
            };

            // Act: call the service to create a new cardholder
            var result = await service.CreateCardholderAsync(dto);

            // Assert: check the returned result
            Assert.NotNull(result); // Result should not be null
            Assert.Equal(dto.FullName, result.FullName); // Name should match input
            Assert.Equal(dto.Email, result.Email); // Email should match input
            Assert.True(result.CardholderId > 0); // ID should be generated and positive
        }

        [Fact]
        public async Task GetAllCardholdersAsync_ShouldReturnAllCardholders()
        {
            // Arrange: create DB and add two cardholders
            var db = GetDbContext();
            db.Cardholders.Add(new Cardholder { FullName = "A", Email = "a@example.com", PasswordHash = "x" });
            db.Cardholders.Add(new Cardholder { FullName = "B", Email = "b@example.com", PasswordHash = "x" });
            await db.SaveChangesAsync();

            var service = new CardholderService(db);

            // Act: retrieve all cardholders
            var result = await service.GetAllCardholdersAsync();

            // Assert: ensure the count matches the number of inserted cardholders
            Assert.Equal(2, result.Count);
        }
    }
}
