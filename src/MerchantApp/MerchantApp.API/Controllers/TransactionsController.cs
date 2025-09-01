using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MerchantApp.API.Data;
using MerchantApp.API.DTOs;
using MerchantApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace MerchantApp.API.Controllers
{
    [ApiController]
    [Route("api/payment")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public TransactionsController(AppDbContext db) => _db = db;

        // Initiate a new payment (creates a pending transaction)
        [HttpPost("initiate")]
        public IActionResult InitiatePayment()
        {
            var merchantId = int.Parse(User.Claims.First(c => c.Type == "merchantId").Value);

            // Get all cart items for this merchant
            var cartItems = _db.CartItems.Where(c => c.MerchantId == merchantId).ToList();
            if (!cartItems.Any())
                return BadRequest("Cart is empty");

            // Create a new transaction
            var transaction = new Transaction
            {
                MerchantId = merchantId,
                TotalAmount = cartItems.Sum(c => c.TotalPrice),
                Status = "Pending",   // Transaction starts as Pending
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddSeconds(45) // Payment timeout (45s)
            };

            _db.Transactions.Add(transaction);
            _db.SaveChanges(); // Save so that TransactionId is generated

            // Copy cart items into TransactionProductDetails
            foreach (var item in cartItems)
            {
                _db.TransactionProductDetails.Add(new TransactionProductDetail
                {
                    TransactionId = transaction.TransactionId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice
                });
            }

            // Clear the cart after transaction is created
            _db.CartItems.RemoveRange(cartItems);
            _db.SaveChanges();

            return Ok(new
            {
                transaction.TransactionId,
                transaction.TotalAmount,
                transaction.Status,
                transaction.ExpiresAt
            });
        }

        // Complete a payment (cardholder provides card details)
        [HttpPost("complete")]
        [AllowAnonymous] // Cardholder is not a merchant, so no auth required
        public IActionResult CompletePayment([FromBody] PaymentConfirmDTO dto)
        {
            // Find the transaction
            var transaction = _db.Transactions
                .Include(t => t.ProductDetails)
                .FirstOrDefault(t => t.TransactionId == dto.TransactionId);

            if (transaction == null) return NotFound("Transaction not found");
            if (transaction.Status != "Pending")
                return BadRequest("Transaction already completed or cancelled.");

            // Check if transaction expired
            if (transaction.ExpiresAt < DateTime.UtcNow)
            {
                transaction.Status = "Timeout";   // Mark as Timeout
                RestoreStock(transaction);        // Restore product stock
                _db.SaveChanges();
                return BadRequest("Transaction expired");
            }

            // Validate card details
            var card = _db.Cards.FirstOrDefault(c =>
                c.CardNumber == dto.CardNumber &&
                c.CVV == dto.CVV &&
                c.ExpiryDate == dto.ExpiryDate &&
                c.Pin == dto.Pin);

            if (card == null) return NotFound("Card not found");

            // Check balance
            if (card.Balance < transaction.TotalAmount)
            {
                transaction.Status = "Failed";   // Insufficient funds
                RestoreStock(transaction);
                _db.SaveChanges();
                return StatusCode(402, "Insufficient balance");
            }

            // Deduct balance and mark as success
            card.Balance -= transaction.TotalAmount;
            transaction.Status = "Success";
            transaction.CardId = card.CardId;
            transaction.UpdatedAt = DateTime.UtcNow;
            _db.SaveChanges();

            return Ok(new
            {
                transaction.TransactionId,
                transaction.Status,
                transaction.TotalAmount,
                PaidAt = transaction.UpdatedAt
            });
        }

        // Check the status of a transaction
        [HttpGet("status/{id}")]
        public IActionResult GetStatus(int id)
        {
            var merchantId = int.Parse(User.Claims.First(c => c.Type == "merchantId").Value);

            var transaction = _db.Transactions
                .Include(t => t.ProductDetails)
                .FirstOrDefault(t => t.TransactionId == id && t.MerchantId == merchantId);

            if (transaction == null) return NotFound("Transaction not found or not owned by you");

            // If still pending but expired → mark as Timeout
            if (transaction.Status == "Pending" && transaction.ExpiresAt < DateTime.UtcNow)
            {
                transaction.Status = "Timeout";
                RestoreStock(transaction);
                _db.SaveChanges();
            }

            return Ok(new
            {
                transaction.TransactionId,
                transaction.Status,
                transaction.TotalAmount,
                transaction.CreatedAt,
                transaction.ExpiresAt
            });
        }

        // Restore product stock if payment fails or times out
        private void RestoreStock(Transaction transaction)
        {
            foreach (var detail in transaction.ProductDetails)
            {
                var product = _db.Products.FirstOrDefault(p => p.ProductId == detail.ProductId);
                if (product != null)
                {
                    product.Stock += detail.Quantity;
                }
            }
        }
    }
}
