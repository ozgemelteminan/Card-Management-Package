# MerchantApp.API Overview README

MerchantApp is a modular system for managing merchants, products, carts, payments, and transactions. It follows a layered architecture to separate concerns and ensure maintainability.

<br>

## 🏗️ Project Structure

### 1️⃣ API Layer
- **Location:** `MerchantApp.API/`
- **Purpose:** Handles HTTP requests, routes, controllers, and request/response models.
- **Responsibilities:**
  - Receive requests from clients
  - Validate input
  - Call Business Layer services
  - Return responses (JSON, status codes)

### 2️⃣ Business Layer / Service
- **Location:** `MerchantApp.Service/`
- **Purpose:** Implements business logic, calculations, and coordination between layers.
- **Responsibilities:**
  - Perform operations like payment processing, cart calculations, stock management
  - Call Data Layer (Repositories) when needed
  - Ensure business rules are enforced

### 3️⃣ Test Layer
- **Location:** `MerchantApp.Tests/`
- **Purpose:** Ensures code correctness through unit and integration tests.
- **Responsibilities:**
  - Test individual services in isolation
  - Validate API endpoints behavior
  - Automate regression checks

<br>

## 🛍️ MerchantApp Module

The **MerchantApp module** is designed as a mobile-focused API for merchants to manage their products, carts, and transactions. It simulates a small payment system, including QR-based payment initiation and status tracking. This module primarily demonstrates layered architecture, API development, and integration with backend services.

### ✨ Key Features
- **🛒 Cart Management:** Merchants can add, remove, or clear items in their cart.
- **📦 Product Management:** Merchants can create new products or update stock.
- **📱 QR Payment Generation:** Initiates a payment transaction and generates a QR code for the customer.
- **⏳ Transaction Status Tracking:** Polls transaction status (Pending / Success / Failed / Timeout).
- **🧪 Test / Extra Endpoints:** Some endpoints exist for testing purposes and do not need to be used in production. They are included to facilitate development and demonstrate service behavior.

### 💡 Notes
- Merchants interact with this API using **JWT authentication**.  
- The QR code feature includes a **timeout mechanism**: transactions pending for more than 45 seconds are automatically marked as `Timeout`.  
- The module interacts with the database for products, transactions, and cart items, but some endpoints are mock/test implementations.  
- Not all endpoints are mandatory; focus on core functionalities like cart management, QR generation, and transaction status for practical purposes.

<br>

## 🚀 Summary
- **API Layer** → Interface clients interact with  
- **Business Layer** → Core logic of the application  
- **Test Layer** → Validates correctness and prevents regressions  

<br>

## 📜 License

This project is intended for **educational and demonstration purposes**.  
© 2025 Özge Meltem İnan — All rights reserved.

