using System;
using System.ComponentModel.DataAnnotations;

namespace CardManagement.Shared.DTOs
{
    public class MerchantLoginRequestDTO
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }


    // Login response for DTO
    public class MerchantLoginResponseDTO
    {
        public string Token { get; set; } = "";
        public int MerchantId { get; set; }
        public string MerchantName { get; set; } = "";
        public DateTime ExpiresAt { get; set; }
    }
    
    
        public class MerchantRegisterDTO
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
