## MobilivaCase

An e-commerce application that manages products and orders. When a customer creates an order, the system automatically sends an email through an asynchronous message queue(RabbitMQ).

## What It Does

MobilivaCase is designed to handle the core operations of an online store:

- Retrieve and display products with caching(using redis) for better performance
- Create customer orders
- Automatically send email confirmations when orders are placed
- Store all data persistently in a database

The application is built as a REST API that can be consumed by frontend applications or other services.

## Technologies Used

**Framework & Language**
- .NET 8.0 - The runtime environment for the application

**Database**
- MySQL 8.0 - Stores products, orders, and customer information
- Entity Framework Core 8.0.5 - ORM layer that handles database operations

**Caching**
- Redis - Caches product data to reduce database queries and improve response times

**Messaging**
- RabbitMQ - Message queue system that handles asynchronous operations like sending emails

**Logging**
- Serilog - Records application events and errors to console and files for debugging and monitoring

**API & Tools**
- AutoMapper - Converts between domain models and API data transfer objects
- Swagger/OpenAPI - Generates interactive API documentation

## How It Works

When a customer places an order:

1. The API receives the order request with customer name, email, and product details
2. The order is saved to the MySQL database
3. The order information is published to a RabbitMQ message queue
4. A background worker service consumes this message
5. The email service sends an email to the customer
6. All operations are logged through Serilog

When requesting products:

1. The API first checks if the products are cached in Redis
2. If cached, it returns them immediately
3. If not cached, it retrieves them from MySQL and stores them in Redis for future requests

## Key Components

**API Server** - Provides REST endpoints for products and orders

**Mail Worker** - Background service that sends emails based on messages from the queue

**Database** - Persists all product and order data

**Cache Layer** - Improves performance by storing frequently accessed data

**Message Queue** - Decouples the API from the email service, allowing asynchronous processing

