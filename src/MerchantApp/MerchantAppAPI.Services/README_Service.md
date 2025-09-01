# MerchantApp Service Layer (drop-in)

This folder contains a minimal service layer that matches your existing API models/DTOs:

- **AuthService / IAuthService** – register + login, returns JWT using `Jwt` section in appsettings.
- **ProductService / IProductService** – CRUD for products restricted to the current merchant.
- **CartService / ICartService** – add/remove/list/clear cart items and reserve/release stock.
- **TransactionService / ITransactionService** – create transaction from cart, confirm payment, query status (handles timeout).
- **QrService / IQrService** – build QR payload and PNG data URL.

## Add to DI

In `Program.cs` add:

```csharp
using MerchantApp.API.Services;
...
builder.Services.AddMerchantAppServices(builder.Configuration);
```

Ensure you have in `appsettings.json`:

```json
"Jwt": {
  "Key": "YOUR_SUPER_SECRET_KEY",
  "Issuer": "MerchantApp",
  "Audience": "MerchantApp.Client",
  "ExpiresMinutes": 120
}
```

## Typical Controller usage

```csharp
[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cart;
    public CartController(ICartService cart) => _cart = cart;

    int MerchantId() => int.Parse(User.Claims.First(c => c.Type == "merchantId").Value);

    [HttpGet]
    public Task<CartDTO> Get() => _cart.GetAsync(MerchantId());

    [HttpPost("{productId:int}/{qty:int}")]
    public Task<CartDTO> Add(int productId, int qty) => _cart.AddItemAsync(MerchantId(), productId, qty);

    [HttpDelete("{productId:int}")]
    public Task<CartDTO> Remove(int productId, [FromQuery] int? qty) => _cart.RemoveItemAsync(MerchantId(), productId, qty);

    [HttpDelete]
    public Task Clear() => _cart.ClearAsync(MerchantId());
}
```

> All services are **async**, return the existing DTOs from your project, and throw `KeyNotFoundException` / `InvalidOperationException` for error cases so your exception middleware can translate them to HTTP codes.

## Notes

- `TransactionService.InitiateAsync` sets `Status = "Pending"` and `ExpiresAt = now + 45s`.
- `ConfirmAsync` validates card, checks balance, deducts, sets `Status = "Success"`, and **clears the cart**.
- `GetStatusAsync` auto-flips `"Pending"` to `"Timeout"` when expired.
- `CartService` locks price at the time of adding to cart by copying `Product.Price` into `CartItem.UnitPrice`.
- Stock is decremented on add and restored on removal/clear.
