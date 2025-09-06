using System.Security.Cryptography;
using System.Text;

namespace MerchantApp.Service.Utils
{
    public static class PasswordHasher
    {
        // Hashes the input string using SHA256.
        // Note: SHA256 is used here for demonstration purposes.
        // For production, use a stronger algorithm like BCrypt or Argon2.
        public static string HashSha256(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes);
        }

        // Verifies whether the SHA256 hash of the input matches the given hash.
        public static bool VerifySha256(string input, string hashHex) =>
            string.Equals(HashSha256(input), hashHex, StringComparison.OrdinalIgnoreCase);
    }
}
