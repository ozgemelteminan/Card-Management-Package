using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CardManagement.Shared.DTOs;
using MerchantApp.Service.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MerchantApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Route: /api/qr
    [Authorize] // Require auth to generate QR (status endpoint is public)
    public class QrController : ControllerBase
    {
        private readonly IQrService _qrService;

        public QrController(IQrService qrService)
        {
            _qrService = qrService;
        }

        private int GetMerchantId() =>
            int.Parse(User.FindFirstValue("merchantId") ?? throw new UnauthorizedAccessException()); // Get merchantId from claims

        [HttpGet("{transactionId:int}")] // Generate a QR code for a transaction
        public async Task<IActionResult> GenerateQr(int transactionId)
        {
            var dto = new QRCodePaymentDTO
            {
                MerchantId = GetMerchantId(),
                TotalAmount = 0, // Optional: client can provide total amount if needed
                ExpireSeconds = 45 // QR expiration time in seconds
            };

            var qr = await _qrService.GenerateQrAsync(dto);
            if (qr == null) return NotFound("Transaction not found or cannot generate QR.");

            return Ok(qr);
        }

        [HttpGet("{transactionId:int}/status")]
        [AllowAnonymous] // endpoint to check QR/payment status (useful for callbacks or public checks)
        public async Task<IActionResult> GetStatus(int transactionId)
        {
            var status = await _qrService.GetStatusAsync(transactionId);
            if (status == null) return NotFound("Transaction not found.");

            return Ok(status);
        }
    }
}
