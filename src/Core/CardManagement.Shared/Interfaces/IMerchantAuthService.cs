using System.Threading.Tasks;
using CardManagement.Shared.Models;

namespace CardManagement.Shared.Interfaces
{
    public interface IMerchantAuthService
    {
        // Sadece login ve register metodları kaldı
        Task<Merchant> RegisterAsync(string name, string email, string password);
        Task<Merchant?> LoginAsync(string email, string password);
    }
}
