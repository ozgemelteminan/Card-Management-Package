using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CardManagement.Data;
using CardManagement.Shared.DTOs;
using CardManagement.Shared.Models;
using QRCoder;
using MerchantApp.Service.Interfaces;

namespace MerchantApp.Service.Services
{
    public class QrService : IQrService
    {
        private readonly AppDbContext _db;

        // Constructor: inject DbContext
        public QrService(AppDbContext db)
        {
            _db = db;
        }

        // Generate a QR code for a pending transaction
        public async Task<QRCodePaymentResponseDTO?> GenerateQrAsync(QRCodePaymentDTO dto)
        {
            // Check if a pending transaction already exists
            var tx = await _db.Transactions
                              .Include(t => t.ProductDetails)
                              .FirstOrDefaultAsync(t => t.MerchantId == dto.MerchantId && t.Status == "Pending");

            if (tx == null)
            {
                // Create a new pending transaction if not exists
                tx = new Transaction
                {
                    MerchantId = dto.MerchantId,
                    TotalAmount = dto.TotalAmount,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddSeconds(dto.ExpireSeconds ?? 45)
                };

                _db.Transactions.Add(tx);
                await _db.SaveChangesAsync();
            }

            // Prepare payload for QR code
            var payloadObj = new
            {
                transactionId = tx.TransactionId,
                amount = tx.TotalAmount,
                issuedAt = DateTime.UtcNow,
                expiresAt = tx.ExpiresAt
            };
            string payload = JsonSerializer.Serialize(payloadObj);

            // Generate QR code using QRCoder
            using var generator = new QRCodeGenerator();
            using var qrData = generator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            var qrPng = new PngByteQRCode(qrData);
            byte[] pngBytes = qrPng.GetGraphic(20);
            string base64 = Convert.ToBase64String(pngBytes);

            // Return DTO with QR code info
            return new QRCodePaymentResponseDTO
            {
                TransactionId = tx.TransactionId,
                QrPayload = payload,
                QrCodeBase64 = base64,
                ExpiresAt = tx.ExpiresAt ?? DateTime.UtcNow
            };
        }

        // Get the status of a transaction by transactionId
        public async Task<QRCodePaymentResponseDTO?> GetStatusAsync(int transactionId)
        {
            var tx = await _db.Transactions
                              .Include(t => t.ProductDetails)
                              .FirstOrDefaultAsync(t => t.TransactionId == transactionId);

            if (tx == null) return null;

            // If transaction is pending but expired, mark as timeout and restore stock
            if (tx.Status == "Pending" && tx.ExpiresAt < DateTime.UtcNow)
            {
                tx.Status = "Timeout";
                RestoreStock(tx);
                await _db.SaveChangesAsync();
            }

            // Return DTO with status info
            return new QRCodePaymentResponseDTO
            {
                TransactionId = tx.TransactionId,
                QrPayload = $"{{\"status\":\"{tx.Status}\"}}",
                ExpiresAt = tx.ExpiresAt ?? DateTime.UtcNow,
                Status = tx.Status
            };
        }

        // Restore stock quantities for a transaction (called when timeout occurs)
        private void RestoreStock(Transaction transaction)
        {
            foreach (var detail in transaction.ProductDetails)
            {
                var product = _db.Products.FirstOrDefault(p => p.ProductId == detail.ProductId);
                if (product != null)
                    product.Stock += detail.Quantity;
            }
        }
    }
}
