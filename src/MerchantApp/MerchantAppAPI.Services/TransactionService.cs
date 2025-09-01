using MerchantApp.API.Data;
using MerchantApp.API.DTOs;
using MerchantApp.API.Models;
using MerchantApp.API.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace MerchantApp.API.Services;

public class TransactionService : ITransactionService
{
    private readonly AppDbContext _db;

    public TransactionService(AppDbContext db) => _db = db;

    public async Task<int> InitiateAsync(int merchantId, CancellationToken ct = default)
    {
        var cartItems = await _db.CartItems
            .Where(c => c.MerchantId == merchantId)
            .ToListAsync(ct);

        if (cartItems.Count == 0) throw new InvalidOperationException("Cart is empty.");

        var now = DateTime.UtcNow;
        var expires = now.AddSeconds(45);

        using var tx = await _db.Database.BeginTransactionAsync(ct);

        var transaction = new Transaction
        {
            MerchantId = merchantId,
            TotalAmount = cartItems.Sum(i => i.TotalPrice),
            Status = "Pending",
            CreatedAt = now,
            ExpiresAt = expires
        };

        await _db.Transactions.AddAsync(transaction, ct);
        await _db.SaveChangesAsync(ct);

        foreach (var i in cartItems)
        {
            var detail = new TransactionProductDetail
            {
                TransactionId = transaction.TransactionId,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.TotalPrice
            };
            await _db.TransactionProductDetails.AddAsync(detail, ct);
        }

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return transaction.TransactionId;
    }

    public async Task<PaymentStatusDTO> ConfirmAsync(PaymentConfirmDTO dto, CancellationToken ct = default)
    {
        var transaction = await _db.Transactions
            .FirstOrDefaultAsync(t => t.TransactionId == dto.TransactionId, ct);

        if (transaction is null) throw new KeyNotFoundException("Transaction not found.");

        if (transaction.Status is "Success")
            return new PaymentStatusDTO { TransactionId = transaction.TransactionId, Status = transaction.Status, TotalAmount = transaction.TotalAmount };

        if (transaction.ExpiresAt.HasValue && DateTime.UtcNow > transaction.ExpiresAt.Value)
        {
            transaction.Status = "Timeout";
            await _db.SaveChangesAsync(ct);
            return new PaymentStatusDTO { TransactionId = transaction.TransactionId, Status = "Timeout", TotalAmount = transaction.TotalAmount };
        }

        // Find matching card
        var card = await _db.Cards.FirstOrDefaultAsync(c =>
            c.CardNumber == dto.CardNumber &&
            c.ExpiryDate == dto.ExpiryDate &&
            c.CVV == dto.CVV, ct);

        if (card is null) throw new InvalidOperationException("Card not found.");

        if (card.Balance < transaction.TotalAmount)
        {
            transaction.Status = "Failed";
            await _db.SaveChangesAsync(ct);
            return new PaymentStatusDTO { TransactionId = transaction.TransactionId, Status = "Failed", TotalAmount = transaction.TotalAmount };
        }

        using var tx = await _db.Database.BeginTransactionAsync(ct);

        // deduct
        card.Balance -= transaction.TotalAmount;
        transaction.CardId = card.CardId;
        transaction.Status = "Success";

        // clear cart for this merchant
        var cartItems = await _db.CartItems.Where(c => c.MerchantId == transaction.MerchantId).ToListAsync(ct);
        _db.CartItems.RemoveRange(cartItems);

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return new PaymentStatusDTO
        {
            TransactionId = transaction.TransactionId,
            Status = transaction.Status,
            TotalAmount = transaction.TotalAmount
        };
    }

    public async Task<PaymentStatusDTO?> GetStatusAsync(int merchantId, int transactionId, CancellationToken ct = default)
    {
        var t = await _db.Transactions.FirstOrDefaultAsync(x => x.TransactionId == transactionId && x.MerchantId == merchantId, ct);
        if (t is null) return null;

        if (t.Status == "Pending" && t.ExpiresAt.HasValue && DateTime.UtcNow > t.ExpiresAt.Value)
        {
            t.Status = "Timeout";
            await _db.SaveChangesAsync(ct);
        }

        return new PaymentStatusDTO { TransactionId = t.TransactionId, Status = t.Status, TotalAmount = t.TotalAmount };
    }
}
