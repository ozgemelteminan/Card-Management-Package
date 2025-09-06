using System.Collections.Generic;
using System.Threading.Tasks;
using CardManagement.Shared.DTOs;

namespace CardManagement.Shared.Interfaces
{
    public interface ITransactionService
    {
        Task<List<CartItemDTO>> GetCartItemsForMerchantAsync(int merchantId);
        Task<StartTransactionResponse> StartTransactionAsync(int merchantId, IReadOnlyList<CartItemDTO> items);
        Task<TransactionStatusDTO?> GetStatusAsync(int transactionId);
        Task<bool> CompletePaymentAsync(PaymentConfirmDTO dto);
        Task<bool> ApproveAsync(int transactionId, int cardId);
        Task<bool> FailAsync(int transactionId, string reason);
        Task<int> TimeoutPendingAsync(int olderThanSeconds = 45);
    }
}
