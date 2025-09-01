namespace Merchant.API.Models
{


    public class MerchantDto
    {
        public int MerchantId { get; set; }                 // İşyeri Id
        public string Name { get; set; } = string.Empty;    // İşyeri adı
        public string Email { get; set; } = string.Empty;   // İşyeri email
        public DateTime CreatedAt { get; set; }             // Kayıt tarihi
    }


    public class LoginRequest
    {
        //Giriş Modeli
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

    }

    public class RegisterMerchantRequest
    {
        //Kayıt Yaptırma Modeli.
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
