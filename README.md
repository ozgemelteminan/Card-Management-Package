# 🧾 MerchantApp.API Documentation

MerchantApp.API is a **.NET 9 App API** for merchants to manage `products`, `carts`, `cards`, and `payments`.  
It supports **JWT authentication**, **Entity Framework Core (SQL Server)**, **FluentValidation**, and provides a **Swagger UI**.


## 📖 Overview

🚀  `MerchantApp.API` provides:
- **Merchant Onboarding & Authentication** (register/login with JWT tokens)
- **Product Management** (create, list, delete merchant products)
- **Cart Management** (add, view, remove items with stock tracking)
- **Card & Cardholder Management**
- **QR Code Payment Initiation & Status Tracking**


## 💡 Example Flow
### 1️⃣ Merchant Registration & Login 🔑
- Merchant registers and logs in
- Receives **JWT token**
> **Endpoint:** `POST /api/auth/register` & `POST /api/auth/login`

---

### 2️⃣ Create Products 🏷️
- Merchant adds products
- Products visible **only to the merchant**
> **Endpoint:** `POST /api/products/add`

---

### 3️⃣ Add Products to Cart 🛒
- Customer adds products to cart
- Stock decreases automatically
> **Endpoint:** `POST /api/cart/add`

---

### 4️⃣ Checkout & Initiate Payment 💳
- Cart is checked out
- Payment initiated
> **Endpoint:** `POST /api/payment/initiate`

---

### 5️⃣ Track Transaction Status 📊
- Monitor payment status
> **Endpoint:** `GET /api/payment/status/{id}`

<br>

## 👨‍💻 Technical Overview

### 🛠 Requirements
- .NET 9 SDK
- SQL Server 
- EF Core CLI (optional)

### 📂 Project Structure
```
MerchantApp.API/
 ├── Controllers/
 │    ├── AuthController.cs
 │    ├── ProductsController.cs
 │    ├── CartController.cs
 │    ├── CardController.cs
 │    ├── CardholderController.cs
 │    ├── TransactionsController.cs
 │    └── QrController.cs
 ├── Program.cs
 ├── appsettings.json
 └── MerchantApp.API.csproj
CardManagement.Data/ (EF Core DbContext + Migrations)
CardManagement.Shared/ (DTOs, Validators, Models)
MerchantApp.Service/ (Service layer)

```

<br>

## ⚙️ Configuration

`appsettings.json` contains the core configuration:

```jsonc
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=Card-Management-System;..."
  },
  "Jwt": {
    "Key": "this-is-a-very-very-long-secret-key-1234567890"
  },
  "AllowedHosts": "*"
}
```

- **JWT**: Issuer & Audience validation disabled by default (set in `Program.cs`)
- **Database**: Adjust `DefaultConnection` to point to your SQL Server

<br>

## 🚀 Setup & Run

```bash
# Restore dependencies
dotnet restore

# Apply migrations (CardManagement.Data project)
dotnet ef database update --project ./Core/CardManagement.Data --startup-project ./MerchantApp.API

# Run API
dotnet run --project ./MerchantApp.API
```

Swagger will be available at: **`https://localhost:5284/swagger`**

<br>

## 🔒 Authentication (JWT)

1. Register → `/api/auth/register`
2. Login → `/api/auth/login` → returns a **JWT token**
3. Pass token in `Authorization: Bearer <token>` header to access protected endpoints

**Login request example**:
```http
POST /api/auth/login HTTP/1.1
Content-Type: application/json

{
  "email": "merchant@example.com",
  "password": "P@ssw0rd!"
}
```

<br>

## 📚  MerchantApp API Endpoints & Swagger Testin
### JWT Authorization

1. Open Swagger UI: `https://localhost:5284/swagger/index.html`
2. Click **Authorize 🔒**
3. Enter your JWT token: `Bearer <your-jwt-token>`

---

### 🔑 AuthController

| HTTP | Path | Action | Parameters |
|---|---|---|---|
| POST | `api/auth/register` | `Register` | `[FromBody] MerchantRegisterDTO dto` |
| POST | `api/auth/login` | `Login` | `[FromBody] MerchantLoginRequestDTO dto` |

**Swagger JSON Examples**

**Register**
```json
{
  "name": "string",
  "email": "user@example.com",
  "password": "string"
}
```

**Login**
```json
{
  "email": "user@example.com",
  "password": "string"
}
```

---

### 💳 CardController

| HTTP | Path | Action | Parameters |
|---|---|---|---|
| POST | `api/cards/create` | `CreateCard` | `[FromBody] CardCreateDTO dto` |
| GET | `api/cards` | `GetAll` | `—` |

**Swagger JSON Example**

```json
{
  "cardholderId": 0,
  "cardNumber": "string",
  "expiryDate": "string",
  "cvv": "string",
  "pin": "string",
  "balance": 0
}
```

---

### 👤 CardholderController

| HTTP | Path | Action | Parameters |
|---|---|---|---|
| POST | `api/cardholders/create` | `CreateCardholder` | `[FromBody] CardholderCreateDTO dto` |
| GET | `api/cardholders` | `GetAll` | `—` |

**Swagger JSON Example**

```json
{
  "fullName": "string",
  "email": "user@example.com",
  "password": "string"
}
```

---

### 🛒 CartController

| HTTP | Path | Action | Parameters |
|---|---|---|---|
| POST | `api/cart/add` | `AddToCart` | `[FromBody] CartItemDTO dto` |
| GET | `api/cart/view` | `ViewCart` | `—` |
| DELETE | `api/cart/remove/{productId}` | `RemoveFromCart` | `productId` |
| DELETE | `api/cart/clear` | `ClearCart` | `—` |

**Swagger JSON Example**

```json
{
  "productId": 0,
  "quantity": 0,
  "unitPrice": 0
}
```

---

### 🏷️ ProductsController

| HTTP | Path | Action | Parameters |
|---|---|---|---|
| POST | `api/Products/add` | `AddProduct` | `[FromBody] ProductCreateDTO dto` |
| GET | `api/Products/view` | `ViewAll` | `—` |
| DELETE | `api/Products/{id}` | `DeleteProduct` | `id` |

**Swagger JSON Example**

```json
{
  "name": "string",
  "price": 0,
  "stock": 0
}
```

---

### 🔲 QrController

| HTTP | Path | Action | Parameters |
|---|---|---|---|
| GET | `api/Qr/{transactionId}` | `GetQr` | `transactionId` |
| GET | `api/Qr/{transactionId}/status` | `CheckQrStatus` | `transactionId` |

---

### 💸 TransactionsController

| HTTP | Path | Action | Parameters |
|---|---|---|---|
| POST | `api/payment/initiate` | `InitiatePayment` | `—` |
| POST | `api/payment/complete` | `CompletePayment` | `[FromBody] PaymentConfirmDTO dto` |
| GET | `api/payment/status/{id}` | `GetPaymentStatus` | `id` |

**Swagger JSON Example**

```json
{
  "transactionId": 0,
  "cardNumber": "string",
  "expiryDate": "string",
  "cvv": "string",
  "pin": "string"
}
```

<br>

## ✅ Validators (FluentValidation)
### 🔑 MerchantLoginDTOValidator
---

| Field    | Constraint                  |
|----------|-----------------------------|
| Email    | Required, valid email format |
| Password | Required                    |


### 🛒 CartItemDTOValidator
---

| Field     | Constraint       |
|-----------|-----------------|
| ProductId | > 0             |
| Quantity  | > 0             |


### 🛒 CartDTOValidator
---

| Field       | Constraint                                         |
|-------------|---------------------------------------------------|
| MerchantId  | > 0                                               |
| TotalAmount | > 0                                               |
| Items       | Must not be empty; each item validated via CartItemDTOValidator |


### 🔲 QRCodePaymentDTOValidator
---

| Field       | Constraint                                         |
|-------------|---------------------------------------------------|
| MerchantId  | > 0                                               |
| TotalAmount | > 0                                               |
| Items       | Must not be empty; validated per item            |


### 💸 PaymentStatusDTOValidator
---

| Field         | Constraint                                |
|---------------|------------------------------------------|
| TransactionId | > 0                                      |
| Status        | Must be one of `Pending | Success | Failed | Timeout` |

<br>

## ⚠️ Error Codes

- `200/201` — Success
- `400` — Validation error / bad request
- `401` — Unauthorized (no/invalid token)
- `403` — Forbidden (not allowed)
- `404` — Resource not found
- `409` — Conflict (e.g., duplicate)
- `500` — Server error

<br>

## 🗄 Database

- Uses **EF Core** with SQL Server
- `AppDbContext` inside **CardManagement.Data**
- Migrations live in **CardManagement.Data**

<br>

## 📜 License

This project is intended for **educational and demonstration purposes**.  
© 2025 Özge Meltem İnan — All rights reserved.
