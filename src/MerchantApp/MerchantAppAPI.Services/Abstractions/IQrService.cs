namespace MerchantApp.API.Services.Abstractions;

public interface IQrService
{
    /// <summary>Returns a data URL (image/png;base64,...) for the QR code of a transaction owned by the merchant.</summary>
    Task<string> GeneratePngDataUrlAsync(int merchantId, int transactionId, CancellationToken ct = default);

    /// <summary>Returns the JSON payload embedded to the QR.</summary>
    Task<string> GetPayloadAsync(int merchantId, int transactionId, CancellationToken ct = default);
}
