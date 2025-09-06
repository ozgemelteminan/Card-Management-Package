using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using CardManagement.Data;
using CardManagement.Shared.DTOs;
using MerchantApp.Service.Services;

namespace MerchantApp.Tests.Tests
{
    public class QrServiceTests
    {
        // Helper to create new in-memory database context for each test
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique DB per test
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task GenerateQrAsync_ShouldCreateTransaction_AndGenerateBase64Qr()
        {
            // Test that generating a QR code creates a transaction and returns a valid Base64 QR payload
            var db = GetDbContext();
            var service = new QrService(db);

            var dto = new QRCodePaymentDTO
            {
                MerchantId = 1,
                TotalAmount = 100,
                Items = new System.Collections.Generic.List<CartItemDTO>(),
                ExpireSeconds = 60
            };

            var result = await service.GenerateQrAsync(dto);

            Assert.NotNull(result); // QR generation result should not be null
            Assert.Equal(100, (await db.Transactions.FirstAsync()).TotalAmount); // Transaction amount should match
            Assert.Equal(result.TransactionId, (await db.Transactions.FirstAsync()).TransactionId); // Transaction ID should match
            Assert.False(string.IsNullOrEmpty(result.QrPayload)); // QR payload should not be empty

            // Check that the QR payload can be converted to Base64
            using var generator = new QRCoder.QRCodeGenerator();
            using var qrData = generator.CreateQrCode(result.QrPayload, QRCoder.QRCodeGenerator.ECCLevel.Q);
            var qrBytes = new QRCoder.PngByteQRCode(qrData).GetGraphic(20);
            var base64 = Convert.ToBase64String(qrBytes);
            Assert.False(string.IsNullOrEmpty(base64));
        }

        [Fact]
        public async Task GetStatusAsync_ShouldReturnCorrectTransactionStatus()
        {
            // Test that the status of an existing transaction is returned correctly
            var db = GetDbContext();
            var service = new QrService(db);

            var tx = new CardManagement.Shared.Models.Transaction
            {
                MerchantId = 1,
                TotalAmount = 50,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5)
            };
            db.Transactions.Add(tx);
            await db.SaveChangesAsync();

            var status = await service.GetStatusAsync(tx.TransactionId);

            Assert.NotNull(status); // Status object should not be null
            Assert.Equal(tx.TransactionId, status.TransactionId); // Transaction ID should match
            Assert.Contains("\"status\":\"Pending\"", status.QrPayload); // Payload should contain the correct status
        }
    }
}