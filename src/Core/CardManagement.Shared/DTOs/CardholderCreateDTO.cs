using System;
using System.ComponentModel.DataAnnotations;

namespace CardManagement.Shared.DTOs
{
    public class CardholderCreateDTO
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!; // Düz şifre, controller'da hashlenir
    }

        public class CardholderResponseDTO
    {

        public int CardholderId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
