using System.Collections.Generic;
using System.Threading.Tasks;
using CardManagement.Shared.DTOs;

namespace MerchantApp.Service.Services
{
    public interface ICardholderService
    {
        Task<CardholderResponseDTO> CreateCardholderAsync(CardholderCreateDTO dto);
        Task<List<CardholderResponseDTO>> GetAllCardholdersAsync();
    }
}
