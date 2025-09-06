using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using CardManagement.Shared.Interfaces;

namespace MerchantApp.API.Controllers
{
    using DTOs = CardManagement.Shared.DTOs;

    [ApiController]
    [Route("api/payment")] // Payment and transaction endpoints
    [Authorize] // Require authentication for most payment operations
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        private int GetMerchantId() =>
            int.Parse(User.Claims.First(c => c.Type == "merchantId").Value); // Read merchantId from JWT claims

        [HttpGet("cart")] // Get cart items for the current merchant (used to prepare a transaction)
        public async Task<IActionResult> GetCartItems()
        {
            var merchantId = GetMerchantId();
            var items = await _transactionService.GetCartItemsForMerchantAsync(merchantId);
            return Ok(items);
        }

        [HttpPost("start")] // Start a transaction using provided cart items
        public async Task<IActionResult> StartTransaction([FromBody] IReadOnlyList<DTOs.CartItemDTO> items)
        {
            var merchantId = GetMerchantId();
            var result = await _transactionService.StartTransactionAsync(merchantId, items);
            return Ok(result);
        }

        [HttpPost("complete")]
        [AllowAnonymous] // Payment gateway callback or public confirmation endpoint
        public async Task<IActionResult> CompletePayment([FromBody] DTOs.PaymentConfirmDTO dto)
        {
            var success = await _transactionService.CompletePaymentAsync(dto);
            if (!success) return BadRequest("Payment failed or invalid transaction");
            return Ok(new { dto.TransactionId, Status = "Success" });
        }

        [HttpGet("status/{transactionId}")] // Get current status of a transaction
        public async Task<IActionResult> GetStatus(int transactionId)
        {
            var status = await _transactionService.GetStatusAsync(transactionId);
            if (status == null) return NotFound("Transaction not found");
            return Ok(status);
        }
    }
}
