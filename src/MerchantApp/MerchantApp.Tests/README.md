# 📦 MerchantApp.Tests

This project contains the **test layer** of the MerchantApp solution.  
Its purpose is to ensure that services behave correctly, business rules are followed, and errors are handled properly.

---

## 📂 Project Structure

```markdown
MerchantApp.Tests/
│
├── MerchantApp.Tests.csproj     # Test project definition
├── ServiceSmokeTests.cs         # General smoke tests
├── Class1.cs                    # Template/example class (kullanılmıyorsa silebilirsin)
└── Tests/                       # Main test directory
    ├── MerchantAuthServiceTests.cs
    ├── QrServiceTests.cs
    ├── CardServiceTests.cs
    ├── TransactionServiceTests.cs
    ├── CartServiceTests.cs
    ├── ProductServiceTests.cs
    └── CardholderServiceTests.cs
```

---

## ⚙️ Setup

Before running the tests, restore dependencies:

```bash
dotnet restore
```

Navigate into the test project directory and run:

```bash
cd src/MerchantApp/MerchantApp.Tests
dotnet test
```

---

## ▶️ Running Tests

Run all tests:
  ```bash
  dotnet test
  ```

Run tests from a specific class:
  ```bash
  dotnet test --filter FullyQualifiedName~MerchantAuthServiceTests
  ```

Run a single test method:
  ```bash
  dotnet test --filter "FullyQualifiedName~MerchantApp.Tests.Tests.TransactionServiceTests.ApproveAsync_ShouldApproveTransaction"
  ```

---

## 🧪 Testing Approach

All tests follow the **Arrange → Act → Assert** pattern:

1. **Arrange:** prepare input data and in-memory DB  
2. **Act:** call the service method  
3. **Assert:** verify results against expectations  

The project uses:

- **xUnit** as the testing framework  
- **EF Core In-Memory Database** for isolation  

---

## ✅ Example Test

```csharp
[Fact]
public async Task CreateCardholderAsync_ShouldCreateCardholder()
{
    // Arrange
    var db = GetDbContext();
    var service = new CardholderService(db);
    var dto = new CardholderCreateDTO
    {
        FullName = "Test User",
        Email = "test@example.com",
        Password = "Password123!"
    };

    // Act
    var result = await service.CreateCardholderAsync(dto);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(dto.Email, result.Email);
    Assert.True(result.CardholderId > 0);
}
```

---

## 📌 Notes

- The project references **MerchantApp.Service**  
- Tests run automatically in Debug configuration  
- Can be integrated into CI/CD pipelines with:  

```bash
dotnet test --logger "trx;LogFileName=test_results.trx"
```
✨ With this test layer, you can validate the reliability of MerchantApp before deployment.
<br>
## 📜 License

This project is intended for **educational and demonstration purposes**.  
© 2025 Özge Meltem İnan — All rights reserved

