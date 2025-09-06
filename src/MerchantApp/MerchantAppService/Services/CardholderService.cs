using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CardManagement.Data;
using CardManagement.Shared.DTOs;
using CardManagement.Shared.Models;

namespace MerchantApp.Service.Services
{
    public class CardholderService : ICardholderService
    {
        private readonly AppDbContext _db;
        private readonly PasswordHasher<Cardholder> _hasher;

        // Constructor: assigns DbContext and initializes PasswordHasher
        public CardholderService(AppDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db)); // Throw if db is null
            _hasher = new PasswordHasher<Cardholder>(); // Initialize password hasher
        }

        public async Task<CardholderResponseDTO> CreateCardholderAsync(CardholderCreateDTO dto)
        {
            // Create new cardholder entity from DTO
            var cardholder = new Cardholder
            {
                FullName = dto.FullName,
                Email = dto.Email,
                CreatedAt = DateTime.UtcNow
            };

            // Hash the password before storing
            cardholder.PasswordHash = _hasher.HashPassword(cardholder, dto.Password);

            // Add cardholder to database and save changes
            _db.Cardholders.Add(cardholder);
            await _db.SaveChangesAsync();

            // Return response DTO with cardholder details
            return new CardholderResponseDTO
            {
                CardholderId = cardholder.CardholderId,
                FullName = cardholder.FullName,
                Email = cardholder.Email,
                CreatedAt = cardholder.CreatedAt
            };
        }

        public async Task<List<CardholderResponseDTO>> GetAllCardholdersAsync()
        {
            // Retrieve all cardholders and map to response DTO
            return await _db.Cardholders
                .Select(c => new CardholderResponseDTO
                {
                    CardholderId = c.CardholderId,
                    FullName = c.FullName,
                    Email = c.Email,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();
        }
    }
}
