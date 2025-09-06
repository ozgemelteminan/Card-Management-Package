using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CardManagement.Data;
using CardManagement.Shared.DTOs;
using CardManagement.Shared.Interfaces;
using CardManagement.Shared.Models;

namespace MerchantApp.Service.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly AppDbContext _db;

        // Constructor: inject DbContext
        public TransactionService(AppDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        // Get all cart items for a specific merchant
        public async Task<List<CartItemDTO>> GetCartItemsForMerchantAsync(int merchantId)
        {
            var items = await _db.CartItems
                .Where(c => c.MerchantId == merchantId)
                .Select(c => new CartItemDTO
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    UnitPrice = c.UnitPrice
                })
                .ToListAsync();

            return items;
        }

        // Start a transaction for a merchant based on cart items
        public async Task<CardManagement.Shared.DTOs.StartTransactionResponse> StartTransactionAsync(int merchantId, IReadOnlyList<CartItemDTO> items)
        {
            if (items == null || items.Count == 0)
                throw new ArgumentException("Cart is empty", nameof(items));

            var productIds = items.Select(i => i.ProductId).ToList();
            var products = await _db.Products
                .Where(p => productIds.Contains(p.ProductId) && p.MerchantId == merchantId)
                .ToListAsync();

            if (products.Count != productIds.Count)
                throw new InvalidOperationException("Some products not found for merchant");

            decimal totalAmount = 0m;

            // Calculate total amount and validate stock
            foreach (var item in items)
            {
                var product = products.First(p => p.ProductId == item.ProductId);

                if (product.Stock < item.Quantity)
                    throw new InvalidOperationException($"Insufficient stock for product {product.Name}");

                totalAmount += item.Quantity * (item.UnitPrice > 0 ? item.UnitPrice : product.Price);
            }

            // Create new transaction
            var transaction = new Transaction
            {
                MerchantId = merchantId,
                TotalAmount = totalAmount,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddSeconds(45)
            };

            _db.Transactions.Add(transaction);
            await _db.SaveChangesAsync();

            // Add transaction product details
            foreach (var item in items)
            {
                var product = products.First(p => p.ProductId == item.ProductId);
                _db.TransactionProductDetails.Add(new TransactionProductDetail
                {
                    TransactionId = transaction.TransactionId,
                    ProductId = product.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice > 0 ? item.UnitPrice : product.Price
                });
            }

            // Clear cart items after starting transaction
            _db.CartItems.RemoveRange(_db.CartItems.Where(c => c.MerchantId == merchantId));
            await _db.SaveChangesAsync();

            return new CardManagement.Shared.DTOs.StartTransactionResponse
            {
                TransactionId = transaction.TransactionId,
                Success = true
            };
        }

        // Get transaction status by transactionId
        public async Task<TransactionStatusDTO?> GetStatusAsync(int transactionId)
        {
            var tx = await _db.Transactions
                .Include(t => t.ProductDetails)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId);

            if (tx == null) return null;

            // Timeout pending transactions if expired
            if (tx.Status == "Pending" && tx.ExpiresAt < DateTime.UtcNow)
                await TimeoutTransactionAsync(tx);

            return new TransactionStatusDTO
            {
                TransactionId = tx.TransactionId,
                Status = tx.Status,
                Reason = null
            };
        }

        // Complete payment using card info
        public async Task<bool> CompletePaymentAsync(PaymentConfirmDTO dto)
        {
            var tx = await _db.Transactions
                .Include(t => t.ProductDetails)
                .FirstOrDefaultAsync(t => t.TransactionId == dto.TransactionId);

            if (tx == null || tx.Status != "Pending")
                return false;

            var card = await _db.Cards.FirstOrDefaultAsync(c =>
                c.CardNumber == dto.CardNumber &&
                c.CVV == dto.CVV &&
                c.ExpiryDate == dto.ExpiryDate &&
                c.Pin == dto.Pin);

            if (card == null || card.Balance < tx.TotalAmount)
            {
                await FailAsync(tx.TransactionId, "Insufficient balance or invalid card");
                return false;
            }

            card.Balance -= tx.TotalAmount;

            // Reduce stock for products
            foreach (var detail in tx.ProductDetails)
            {
                var product = await _db.Products.FirstOrDefaultAsync(p => p.ProductId == detail.ProductId);
                if (product != null)
                    product.Stock -= detail.Quantity;
            }

            tx.Status = "Success";
            tx.CardId = card.CardId;
            tx.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        // Approve transaction manually with card
        public async Task<bool> ApproveAsync(int transactionId, int cardId)
        {
            var card = await _db.Cards.FirstOrDefaultAsync(c => c.CardId == cardId);
            if (card == null) return false;

            var tx = await _db.Transactions
                .Include(t => t.ProductDetails)
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId);

            if (tx == null || tx.Status != "Pending") return false;

            if (card.Balance < tx.TotalAmount)
                return await FailAsync(transactionId, "Insufficient balance");

            card.Balance -= tx.TotalAmount;

            foreach (var detail in tx.ProductDetails)
            {
                var product = await _db.Products.FirstOrDefaultAsync(p => p.ProductId == detail.ProductId);
                if (product == null) return await FailAsync(transactionId, "Product not found");
                if (product.Stock < detail.Quantity) return await FailAsync(transactionId, "Insufficient stock");

                product.Stock -= detail.Quantity;
            }

            tx.Status = "Success";
            tx.CardId = card.CardId;
            tx.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        // Fail a transaction and restore stock
        public async Task<bool> FailAsync(int transactionId, string reason)
        {
            var tx = await _db.Transactions
                .Include(t => t.ProductDetails)
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId);

            if (tx == null || tx.Status != "Pending") return false;

            foreach (var detail in tx.ProductDetails)
            {
                var product = await _db.Products.FirstOrDefaultAsync(p => p.ProductId == detail.ProductId);
                if (product != null)
                    product.Stock += detail.Quantity;
            }

            tx.Status = "Failed";
            tx.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        // Timeout all pending transactions older than a threshold
        public async Task<int> TimeoutPendingAsync(int olderThanSeconds = 45)
        {
            var threshold = DateTime.UtcNow.AddSeconds(-olderThanSeconds);
            var pending = await _db.Transactions
                .Include(t => t.ProductDetails)
                .Where(t => t.Status == "Pending" && t.CreatedAt < threshold)
                .ToListAsync();

            foreach (var t in pending)
                await TimeoutTransactionAsync(t);

            return pending.Count;
        }

        // Private helper to timeout a transaction
        private async Task TimeoutTransactionAsync(Transaction tx)
        {
            tx.Status = "Timeout";
            tx.UpdatedAt = DateTime.UtcNow;

            foreach (var detail in tx.ProductDetails)
            {
                var product = await _db.Products.FirstOrDefaultAsync(p => p.ProductId == detail.ProductId);
                if (product != null)
                    product.Stock += detail.Quantity;
            }

            await _db.SaveChangesAsync();
        }
    }
}
