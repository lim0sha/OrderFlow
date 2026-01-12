# OrderFlow 📦
A comprehensive order and product management service with gRPC API, HTTP gateway, and event-driven integration via Kafka.

---

## Features

* Product management: create products, paginated search, filtering by ID, price range, and name.
* Order management: create orders, update status, add/remove products in orders (with rules for allowed operations).
* Order item management: add/remove items (soft delete), paginated search, filtering.
* Order history management: track all changes, polimorphic serialization/deserialization, paginated search.
* gRPC API with oneof-polymorphism.
* HTTP gateway with Swagger documentation and polymorphic models.
* Event-driven integration via Kafka (`order_creation`, `order_processing`).
* Full transaction history for all operations.
* Hexagonal architecture for clean separation of business logic, repository, and presentation layers.

---

## Technologies Used

### Backend

* **C# 12** (.NET 8)
* **ASP.NET Core** for HTTP API and gRPC services
* **Npgsql** for PostgreSQL database access
* **FluentMigrator** for database migrations
* **Microsoft.Extensions.Options** for configuration
* **Kafka** for event-driven inter-service communication
* **Docker** for containerization

### Frontend / Presentation

* **gRPC** for API services
* **HTTP API** with Swagger/OpenAPI documentation
* **Middleware** for error handling and mapping gRPC exceptions to HTTP codes

### Testing & Development

* **Console application** for scenario testing
* **Docker Compose** for multi-service local environment including separate database and Kafka
* **Transactions** in service layer for multi-step operations

---

## How It Works

OrderFlow manages products and orders with the following workflow:

1. Load initial configurations via configuration service.
2. Create several products.
3. Create an order.
4. Add products to the order.
5. Remove a product from the order.
6. Move the order to `processing`.
7. Complete the order.
8. Output full order history.

All operations are logged in the order history with polymorphic serialization for accurate event tracking.

The service communicates with Kafka topics:

* `order_creation` – messages about new orders or status changes.
* `order_processing` – incoming messages from the order processing service, updating statuses and history accordingly.

---


## Getting Started

### Prerequisites

* .NET 8 SDK
* PostgreSQL
* Docker & Docker Compose
* Kafka

### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/username/OrderFlow.git
   ```

2. Navigate to the project directory:

   ```bash
   cd OrderFlow/src/lab-4/DataAccess/DataAccess
   ```


3. Start DataAccess layer via Docker Compose:

   ```bash
   docker-compose up -d
   ```

5. Start API-Gateway layer via Docker Compose:

   ```bash
   cd ../..
   cd OrderFlow/src/lab-4/Gateway/Gateway
   dotnet run
   ```
6. Start Kafka and Presentation layer via Docker Compose:

    ```bash
   cd ../..
   cd OrderFlow/src/lab-4/Presentation/Presentation
   docker-compose up -d
   dotnet run
   ```

8. Open Swagger UI at `http://localhost:5000/swagger/index.html` to explore HTTP endpoints.

---

## API Documentation

* **gRPC API**: full order management and product management operations with polymorphic models.
* **HTTP Gateway**: REST endpoints reflecting gRPC functionality with Swagger documentation.

---

## License

This project is licensed under the MIT License – see [LICENSE.md](docs/src/LICENSE.md) for details. 
Initial project structure and functional requirements are provided [here](docs/src).

## Contact

For questions or feedback, contact me at [Limosha@inbox.ru](mailto:Limosha@inbox.ru).

---