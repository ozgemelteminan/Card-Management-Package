# MerchantApp.API

MerchantApp.API is a sample **.NET 7 Web API** project that simulates a merchant payment system. It includes:
- Merchant authentication (JWT-based)
- Product management
- Cart operations (intended for testing only) 
- QR Code payment flow 
- Cardholder and card management (intended for testing only)
- Transaction handling
- Validation with **FluentValidation**

---

## 📌 Requirements
- .NET 7 SDK
- Visual Studio Code or Visual Studio

---

## 🚀 Setup & Run
```bash
dotnet restore
dotnet build
dotnet run
```
The API will run on:
```
http://localhost:5284
```
>This is an example, you should paste the URL that appears in your terminal.

Swagger UI:
```
http://localhost:5284/swagger/index.html
```

---

## 🔑 Authentication
- Merchants must register and login to receive a JWT token.
- JWT token must be included in requests to protected endpoints:
```http
Authorization: Bearer <your_token>
```

---

## 📂 Endpoints

### 1. **AuthController** (`/api/auth`)
- **POST** `/register` → Register a merchant
- **POST** `/login` → Login and receive JWT token

### 2. **ProductsController** (`/api/products`)
- **POST** `/add` → Add product (or increase stock if exists)
- **GET** `/view` → View merchant’s products
- **DELETE** `/{id}` → Delete product

### 3. **CartController** (`/api/cart`)
- **POST** `/add` → Add product to cart
- **GET** `/view` → View cart items
- **DELETE** `/remove/{productId}` → Remove product from cart
- **DELETE** `/clear` → Clear cart

### 4. **CardholdersController** (`/api/cardholders`)
- **POST** `/create` → Create cardholder
- **GET** `/` → List all cardholders

### 5. **CardsController** (`/api/cards`)
- **POST** `/create` → Create a card for cardholder
- **GET** `/` → List all cards

### 6. **TransactionsController** (`/api/payment`)
- **POST** `/initiate` → Initiate payment (creates transaction from cart)
- **POST** `/complete` → Complete payment (with card details)
- **GET** `/status/{id}` → Get transaction status

### 7. **QrController** (`/api/qr`)
- **GET** `/generate/{transactionId}` → Generate QR code for transaction

---

## 🛡️ Validators (FluentValidation)

### CartDTOValidator
- MerchantId must be greater than 0  
- Cart cannot be empty  

### CartItemDTOValidator
- ProductId must be greater than 0  
- Quantity must be greater than 0  

### MerchantLoginDTOValidator
- Email must not be empty and valid format  
- Password must not be empty and at least 6 characters  

### PaymentStatusDTOValidator
- TransactionId must be greater than 0  
- Status must be one of: `Pending`, `Success`, `Failed`, `Timeout`  

### QRCodePaymentDTOValidator
- MerchantId must be greater than 0  
- TotalAmount must be greater than 0  
- Cart cannot be empty  
- Each CartItem validated with `CartItemDTOValidator`  

---

## 📊 Payment Flow
1. Merchant adds products.
2. Merchant adds products to cart.
3. Initiate payment → transaction created.
4. QR code is generated for transaction.
5. Cardholder scans QR → completes payment. (Cardholder App)
6. Merchant can check status.

---

## 📜 License
This project is for **educational/demo purposes**.  
