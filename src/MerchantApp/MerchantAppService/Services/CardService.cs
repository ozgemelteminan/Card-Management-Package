using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; 
using CardManagement.Data;
using CardManagement.Shared.DTOs;
using CardManagement.Shared.Models;

namespace MerchantApp.Service.Services
{
    public class CardService : ICardService
    {
        private readonly AppDbContext _db;

        // Constructor: inject the database context
        public CardService(AppDbContext db)
        {
            _db = db; // Assign DbContext
        }

        // Creates a new card for a cardholder
        public async Task<Card> CreateCardAsync(CardCreateDTO dto)
        {
            // Find the cardholder by ID
            var cardholder = await _db.Cardholders
                .FirstOrDefaultAsync(c => c.CardholderId == dto.CardholderId);

            if (cardholder == null)
                throw new KeyNotFoundException("Cardholder not found"); // Throw if cardholder doesn't exist

            // Map DTO to Card entity
            var card = new Card
            {
                CardholderId = dto.CardholderId,
                CardNumber = dto.CardNumber,
                ExpiryDate = dto.ExpiryDate,
                CVV = dto.CVV,
                Pin = dto.Pin,
                Balance = dto.Balance
            };

            // Add the card to the database
            _db.Cards.Add(card);
            await _db.SaveChangesAsync(); // Save changes asynchronously

            return card; // Return the created card
        }

        // Retrieves all cards from the database
        public async Task<List<Card>> GetAllCardsAsync()
        {
            return await _db.Cards.ToListAsync(); // Execute query asynchronously and return list
        }
    }
}
