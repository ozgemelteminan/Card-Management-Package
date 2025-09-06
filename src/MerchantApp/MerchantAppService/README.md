# 📦 MerchantApp.Service

**MerchantApp.Service** contains the core business logic (service layer) for the MerchantApp solution.  
It implements service classes used by the Web API and other layers, handles validation, transactions, and coordinates persistence through the shared `AppDbContext`.

<br>

## 📂 Project Overview

This project exposes a set of services registered to DI that implement application business rules such as authentication, card management, cart/checkout flows, transactions, and product operations. It relies on the shared `CardManagement.*` projects (Data, DTOs, Models, Interfaces).

<br>

## 📂 Project Structure

```
MerchantApp.Service/
├── MerchantApp.Service.csproj        # Project file
├── README.md                         # documentation
├── Extensions/
│   └── ServiceCollectionExtensions.cs  # DI registration helpers
├── Services/
│   ├── MerchantAuthService.cs        # Merchant authentication & password verification
│   ├── CardholderService.cs          # Cardholder CRUD + queries
│   ├── CardService.cs                # Card creation / lookup
│   ├── CartService.cs                # Cart management, totals, validation
│   ├── ProductService.cs             # Product lookup / stock
│   ├── TransactionService.cs         # Transaction lifecycle (create, approve, timeout)
│   └── ...                           # other services
├── Utils/
│   └── PasswordHasher.cs             # helpers (SHA256) and other utilities
├── bin/ obj/                          # build outputs and intermediate files
└── .vs/ .DS_Store                     # IDE and OS metadata (ignore)
```

<br>

## ✅ Services & Responsibilities

🔐 **MerchantAuthService**  
  Handles login, password verification (`PasswordHasher`), and merchant lookup.  

👤 **CardholderService**  
  Manages cardholder CRUD operations, including password hashing via `PasswordHasher<Cardholder>` (Identity).  

💳 **CardService**  
  Provides card creation and retrieval logic.  

🛒 **CartService**  
  Supports adding/removing items, validating stock/pricing, and computing totals.  

📦 **ProductService**  
  Handles product queries and stock checks.  

💰 **TransactionService**  
  Creates transactions, processes payment approval/denial, and manages timeouts/status updates.  

⚙️ **ServiceCollectionExtensions**  
  Exposes `AddMerchantAppServices(this IServiceCollection services)` to register all services into the DI container.


<br>

## ⚙️ Prerequisites

- .NET SDK 9.0 (or the target framework specified in `MerchantApp.Service.csproj`)  
- Shared projects required by this service:
  - `CardManagement.Data` (contains `AppDbContext`)  
  - `CardManagement.Shared.DTOs`, `CardManagement.Shared.Models`, `CardManagement.Shared.Interfaces`  
- A configured database provider (EF Core). In tests/in-memory scenarios EF Core InMemory is used.

<br>

## 🔧 Installation & Local Setup

1. Clone the monorepo or place this project inside your solution so the shared projects are referenced.
2. Restore packages:
   ```bash
   dotnet restore
   ```
3. Ensure project references in `MerchantApp.Service.csproj` point to the correct shared projects (relative paths).

4. Build the project:
   ```bash
   dotnet build
   ```

<br>

## 🧩 Registering Services (Dependency Injection)

Add service registration to your `Program.cs` or the composition root:

```csharp
using MerchantApp.Service.Extensions;

var builder = WebApplication.CreateBuilder(args);

// register DbContext (example)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// register merchant services
builder.Services.AddMerchantAppServices();

var app = builder.Build();
```

`AddMerchantAppServices()` wires up scoped services such as `IMerchantAuthService`, `ICardService`, `ICardholderService`, `IProductService`, `ICartService`, `ITransactionService`, etc.

<br>

## 🧪 Testing

- Unit tests should mock or use an in-memory `AppDbContext` (EF Core InMemory) to isolate the service layer.
- Services are written to be testable — prefer constructor injection for dependencies (DbContext, loggers, etc).
- Example: create an in-memory `DbContextOptions<AppDbContext>` and pass it to `new AppDbContext(options)` in tests.

<br>

## 🔍 Key Implementation Notes

| Area                  | Details                                                                 |
|-----------------------|-------------------------------------------------------------------------|
| 🔑 Password Hashing   | Uses `PasswordHasher` (SHA256). For production, prefer PBKDF2/bcrypt.   |
| 🔄 Transactions       | Ensure DB transactions to avoid race conditions.                       |
| ⚠️ Exception Handling | Map service exceptions to proper HTTP codes (400/422/409/500).          |
| 🛡 Validation         | API/DTO input validation + service-level invariant checks.              |
| 📝 Logging            | Use `ILogger<T>` for login failures, stock shortages, denials.          |

---

## ✅ Example Usage (service-level)

```csharp
// Resolve from DI: ICardService _cardService
var createdCard = await _cardService.CreateCardAsync(new CardCreateDTO { /* ... */ });

// Merchant login
var merchant = await merchantAuthService.LoginAsync(email, password);
if (merchant == null) { /* authentication failed */ }

// Create transaction via TransactionService
var txDto = new CreateTransactionDTO { /* cart items, merchant id, etc. */ };
var tx = await transactionService.CreateTransactionAsync(txDto);
```

---

## 📜 License

This project is intended for **educational and demonstration purposes**.  
© 2025 Özge Meltem İnan — All rights reserved.


