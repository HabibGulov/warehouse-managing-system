# Warehouse Management System

## Project Description

This project is a **web application** developed using **.NET 8** and **XML** for data storage, designed to efficiently manage a warehouse. Users can track products, orders, and suppliers, optimizing the inventory management process.

---

## Key Features

### Products
- **Manage product information** such as:
  - **ID**: Unique identifier for each product.
  - **Name**: The name of the product.
  - **Description**: A brief description of the product.
  - **Quantity**: Available stock quantity.
  - **Price**: The cost of the product.
  - **Category ID**: Links to the associated product category.

### Categories
- **Organize products** into categories, allowing for better management and reporting. Each category includes:
  - **ID**: Unique identifier for each category.
  - **Name**: The name of the category (e.g., electronics, clothing).
  - **Description**: A brief overview of what products are included in this category.

### Suppliers
- **Maintain information** about suppliers, which includes:
  - **ID**: Unique identifier for each supplier.
  - **Name**: The name of the supplier.
  - **Contact Person**: The representative to contact.
  - **Email**: Email address for communication.
  - **Phone**: Contact number.

### Orders
- **Monitor orders** placed for products, which include:
  - **ID**: Unique identifier for each order.
  - **Product ID**: The product being ordered.
  - **Quantity**: Amount of the product ordered.
  - **Order Date**: When the order was placed.
  - **Supplier ID**: Links to the supplier who fulfilled the order.
  - **Status**: Current status of the order (e.g., in process, delivered).

### Age Verification Middleware
- The **`AgeCheck` middleware** ensures that only users of a certain age can access specific resources, enhancing the security of the application.

---

## Technical Features

- **Dependency Injection (DI)**: Simplifies dependency management in the application, making the code cleaner and more maintainable.
- **Middleware checking client age**: Server-level error handling to ensure application resilience, including the `AgeCheck` middleware for age verification.
- **API Conventions**: Standardized API methods for increased usability and consistency.
- **Extension Methods**: Simplify working with API results.

---
