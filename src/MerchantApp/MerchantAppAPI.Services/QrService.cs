using MerchantApp.API.Data;
using MerchantApp.API.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System.Text.Json;

namespace MerchantApp.API.Services;

public class QrService : IQrService
{
    private readonly AppDbContext _db;

    public QrService(AppDbContext db) => _db = db;

    public async Task<string> GetPayloadAsync(int merchantId, int transactionId, CancellationToken ct = default)
    {
        var t = await _db.Transactions.FirstOrDefaultAsync(x => x.TransactionId == transactionId && x.MerchantId == merchantId, ct);
        if (t is null) throw new KeyNotFoundException("Transaction not found or not owned by merchant.");
        var payload = new { transactionId = t.TransactionId };
        return JsonSerializer.Serialize(payload);
    }

    public async Task<string> GeneratePngDataUrlAsync(int merchantId, int transactionId, CancellationToken ct = default)
    {
        var payload = await GetPayloadAsync(merchantId, transactionId, ct);
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
        using var qr = new PngByteQRCode(data);
        var bytes = qr.GetGraphic(20);
        return $"data:image/png;base64,{Convert.ToBase64String(bytes)}";
    }
}
