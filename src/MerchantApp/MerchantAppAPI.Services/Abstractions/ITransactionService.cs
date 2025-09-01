using MerchantApp.API.DTOs;

namespace MerchantApp.API.Services.Abstractions;

public interface ITransactionService
{
    /// <summary>Creates a transaction from the current cart and returns its id.</summary>
    Task<int> InitiateAsync(int merchantId, CancellationToken ct = default);

    /// <summary>Confirms a payment (simulates card payment) and returns final status.</summary>
    Task<PaymentStatusDTO> ConfirmAsync(PaymentConfirmDTO dto, CancellationToken ct = default);

    /// <summary>Gets current status for a transaction. Also flips to Timeout if expired.</summary>
    Task<PaymentStatusDTO?> GetStatusAsync(int merchantId, int transactionId, CancellationToken ct = default);
}
