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
    public class CardServiceTests
    {
        // Helper method to create a unique in-memory database context per test
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique DB for isolation
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task CreateCardAsync_ShouldCreateCard()
        {
            // Arrange: create DB, add a cardholder, and initialize the service
            var db = GetDbContext();
            var cardholder = new Cardholder { FullName = "User1", Email = "u1@example.com", PasswordHash = "x" };
            db.Cardholders.Add(cardholder);
            await db.SaveChangesAsync();

            var service = new CardService(db);

            var dto = new CardCreateDTO
            {
                CardholderId = cardholder.CardholderId,
                CardNumber = "1111222233334444",
                ExpiryDate = "12/30",
                CVV = "123",
                Pin = "0000",
                Balance = 100
            };

            // Act: create a new card
            var result = await service.CreateCardAsync(dto);

            // Assert: verify the card is created correctly
            Assert.NotNull(result);
            Assert.Equal(dto.CardNumber, result.CardNumber);
            Assert.Equal(dto.Balance, result.Balance);
            Assert.Equal(dto.CVV, result.CVV);
            Assert.Equal(dto.ExpiryDate, result.ExpiryDate);
            Assert.Equal(dto.Pin, result.Pin);
        }

        [Fact]
        public async Task GetAllCardsAsync_ShouldReturnAllCards()
        {
            // Arrange: create DB and add multiple cards
            var db = GetDbContext();
            db.Cards.Add(new Card { CardNumber = "1", Balance = 10, CVV = "111", ExpiryDate = "12/25", Pin = "0000" });
            db.Cards.Add(new Card { CardNumber = "2", Balance = 20, CVV = "222", ExpiryDate = "11/24", Pin = "1234" });
            await db.SaveChangesAsync();

            var service = new CardService(db);

            // Act: retrieve all cards
            var result = await service.GetAllCardsAsync();

            // Assert: verify the retrieved data
            Assert.Equal(2, result.Count); // Should return 2 cards
            Assert.All(result, c => Assert.False(string.IsNullOrEmpty(c.CVV))); // CVV must be set
            Assert.All(result, c => Assert.False(string.IsNullOrEmpty(c.ExpiryDate))); // Expiry date must be set
            Assert.All(result, c => Assert.False(string.IsNullOrEmpty(c.Pin))); // PIN must be set
        }
    }
}
