# WebApiShop Microservices Architecture Plan

## 1. Executive Summary
Current state: WebApiShop is a layered monolith (API -> Service -> Repositeries -> SQL Server via EF Core).

Target state: Domain-oriented microservices with clear ownership boundaries, independent deployment, and gradual migration using the Strangler pattern.

Scope of this file: Architecture planning only. No implementation is required in this assignment.

## 2. Service Decomposition (DDD)
The decomposition follows existing business domains in the current codebase.

### 2.1 Proposed Core Services

| Service | Bounded Context | Current Source Areas | Responsibilities |
|---|---|---|---|
| Identity Service | Identity and Access | UsersController, UsersPasswordController, UserService, UserPasswordService | Registration, login, password policy, user profile basics |
| Catalog Service | Product Catalog | ProductsController, CategoriesController, ProductService, CategoryService | Product CRUD, category CRUD, filtering, sorting, pagination |
| Orders Service | Order Management | OrdersController, OrderService | Create orders, order items, status transitions, order sum validation |
| Ratings Service | Analytics and Tracking | RatingMiddleware, RatingService | Request tracking ingestion and reporting endpoints |

### 2.2 Supporting Services

| Service | Purpose |
|---|---|
| API Gateway | Single entry point, routing, rate limiting, auth passthrough |
| Notification Service | Email workflows and event-based notifications |

## 3. Communication Strategy

### 3.1 Synchronous Communication (Phase 1)
- Client -> API Gateway -> internal services via REST.
- Orders Service calls Catalog Service for product/stock/price checks.
- Orders Service calls Identity Service for user validation.

### 3.2 Asynchronous Communication (Phase 2)
- Introduce message broker for domain events.
- Initial event candidates:
  - UserRegistered
  - OrderCreated
  - OrderStatusChanged
- Notification Service subscribes to events instead of direct calls.

## 4. Data Strategy (Database per Service)

| Service | Owned Data |
|---|---|
| Identity Service | Users, UserPassword |
| Catalog Service | Products, Categories |
| Orders Service | Orders, OrdersItems |
| Ratings Service | Ratings |

Rules:
- Each service owns its tables/schema.
- No direct cross-service table access.
- Cross-service reads are done via APIs or replicated read models.

## 5. API Gateway and Cross-Cutting Concerns
Gateway responsibilities:
- Route by path to internal services.
- Enforce authentication and authorization policy.
- Apply rate limiting and request correlation id.

Standardized concerns across all services:
- Unified error response format.
- HTTPS and CORS policy.
- Health endpoints (live, ready).
- Centralized logs and distributed tracing.

## 6. Migration Roadmap (Strangler Pattern)

### Phase 1: Foundation
- Keep monolith as source of truth.
- Add API Gateway in front of existing monolith.
- Define shared contracts for service-to-service communication.

### Phase 2: Extract Catalog Service
- Move product/category endpoints first (low transaction risk).
- Validate filtering, sorting, pagination parity with monolith.

### Phase 3: Extract Identity Service
- Move registration, login, password flows.
- Replace direct user access in other domains with service APIs.

### Phase 4: Extract Orders Service
- Move order creation and status flows.
- Keep server-side order sum validation in Orders Service.

### Phase 5: Extract Ratings Service
- Move tracking ingestion from monolith middleware path.
- Expose analytics/reporting endpoints.

### Phase 6: Add Notification Service (Optional)
- Start with order confirmation emails.
- Trigger notifications from events.

## 7. Risks and Mitigations

| Risk | Mitigation |
|---|---|
| Distributed transaction complexity | Prefer eventual consistency and idempotent handlers |
| Service-to-service network failures | Timeouts, retries with backoff, circuit breaker policy |
| Harder debugging in distributed flows | Correlation ids, centralized logs, tracing |
| Data duplication between domains | Clear ownership and event-driven synchronization contracts |

## 8. Success Criteria
- Planning document exists under .github.
- Service boundaries are clear and map to current code domains.
- Migration path is phased and low risk.
- Document is understandable without reading source code first.

## 9. Non-Goals (For This Homework)
- No code implementation of microservices.
- No physical database split yet.
- No container orchestration setup yet.
- No CI/CD migration in this assignment.
