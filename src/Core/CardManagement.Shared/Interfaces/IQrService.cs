using System.Threading.Tasks;
using CardManagement.Shared.DTOs;

namespace MerchantApp.Service.Interfaces
{
    public interface IQrService
    {
        Task<QRCodePaymentResponseDTO?> GenerateQrAsync(QRCodePaymentDTO dto);
        Task<QRCodePaymentResponseDTO?> GetStatusAsync(int transactionId);
    }
}
