using System.Collections.Generic;
using System.Threading.Tasks;
using CardManagement.Shared.DTOs;
using CardManagement.Shared.Models;

namespace MerchantApp.Service.Services
{
    public interface ICardService
    {
        Task<Card> CreateCardAsync(CardCreateDTO dto);
        Task<List<Card>> GetAllCardsAsync();
    }
}
