# 💍 WebApiShop - Jewelry Store System (REST API)

An advanced Backend system for managing a virtual jewelry store, implemented as a modern **REST API** using **ASP.NET Core 9** and **C#**.
The system was built with a focus on REST principles, high performance, scalability, and a clear separation of layers (Decoupling).

## 🏗 Architecture and Project Structure

The project is built using a **3-layer (N-Tier Architecture)**, which ensures easy maintenance, clean code, and effective testing:

1. **Application Layer (Web API):**

   * Manages requests (Controllers) and defines Routing configurations.
   * Implements custom Middlewares (errors, tracking).
   * Centralized Dependency Injection (DI) setup.

2. **Services Layer (Business Logic):**

   * Acts as an intermediary between Controllers and Repositories.
   * Contains all the business logic of the jewelry store (e.g., validating order price).
   * Operations are performed **asynchronously** to free up server resources (Threads).

3. **Repositories Layer (Data Access):**

   * Database access through Entity Framework Core.
   * Works with **Database First** approach.
   * Data retrievals and updates are done asynchronously for performance improvement and scalability.

## 🛠 Technologies and Key Highlights

### ⚡ Performance and Dependency Decoupling

* **Asynchronous Programming:** Uses `async/await` throughout the project to release Threads and improve Scalability.
* **Dependency Injection:** Injects dependencies between layers to create full decoupling between interfaces and implementations.

### 🔄 Data Transfer and Mapping (DTOs)

* **DTO (Data Transfer Object):** A dedicated DTO layer to avoid circular dependencies and prevent exposing database entities externally (without using `[JsonIgnore]`).
* **C# Records:** DTO objects are implemented as `records`, ideal for transferring immutable data.
* **AutoMapper:** Uses the AutoMapper library for clean and efficient automatic mapping between Entities and DTOs.

### 📊 Monitoring, Logs, and Error Handling

* **NLog:** Advanced logging system that stores information (Information and above) in dedicated log files.
* **Error Handling Middleware:** Global and uniform error handling for monitoring and preventing server crashes.
* **Rating Middleware:** All system traffic and requests are logged and stored in a dedicated `Rating` table for tracking and statistics.
* **Configuration:** All system settings (e.g., connection strings) are stored separately from the code in the `appsettings.json` file.
* **Security:** The system operates under the HTTPS protocol.

## 🧪 Testing

The project places a strong emphasis on code quality and reliability, with extensive automated testing coverage (over 12 tests):

* **Unit Tests:** Isolated tests for the Services layer (e.g., `OrderServiceUnitTests`) and the Repositories layer. Includes comprehensive tests for order sum logic (Happy Path and Unhappy Path).
* **Integration Tests:** Full test suite for all Repositories (`Category`, `Order`, `Product`, `User`) to verify communication with the database.
* **Database Fixture:** Uses an advanced infrastructure to share database connections between tests.

## 📂 Folder Structure in Solution

```text
├── WebApiShop (Application) # Controllers, Middlewares, AppSettings
├── Services                 # Business Logic, AutoMapper Profiles, DTOs (Records)
├── Repositories             # DB Context, Entities (EF), Repository Implementations
└── Tests                    # Unit Tests, Integration Tests, Database Fixture
```

---

