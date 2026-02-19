# Architecture & Design Notes — Chuks Kitchen Backend

This document explains the backend flows, edge-case handling, assumptions, and scalability considerations for the Chuks Kitchen project. It complements the code and PlantUML diagrams in `diagrams/`.

---
## 1) Flow explanations (step‑by‑step)

Signup & Verify (see `diagrams/flows.puml`)
- User fills signup form (email or phone, password, confirmPassword, optional referral).
- Frontend calls `POST /api/users/signup` (controller -> `UserService.SignupAsync`).
  - Service checks duplicates via `IUserRepository.GetByEmailOrPhoneAsync`.
  - Referral code validated via `ReferralService.IsValidAsync` (simple rule: `TRUEMINDS`).
  - A random 4-digit OTP is generated and stored in the `User` record with an expiry (5 minutes).
  - Password is stored per the project instruction (plain text in `PasswordHash` property).
  - User is saved to DB (SQLite via `ApplicationDbContext`).
  - For demo/testing the generated OTP is returned in the signup response.
- User submits OTP to `POST /api/users/verify` (`UserService.VerifyAsync`): server checks OTP value and expiry, sets `IsVerified=true`.

Cart → Order (see `diagrams/order_flow.puml`)
- Customer browses foods: `GET /api/foods` -> `FoodRepository.GetAllAsync()` returns only available items (`IsAvailable == true`).
- Add to cart: `POST /api/carts/add` -> `CartService.AddToCartAsync` / `CartRepository` creates cart and `CartItem`s or updates quantity.
- Place order: `POST /api/orders` -> `OrderService.CreateAsync`:
  - Validates each item exists and is available via `IFoodRepository.GetByIdAsync`.
  - Calculates total price and creates `Order` and `OrderItem` entries.
  - Sets lifecycle status initially to `Pending` (OrderStatus enum).
  - Saves order in DB via `IOrderRepository`.
  - Returns created order with status.

Admin food management (see `diagrams/admin_flow.puml`)
- Admin logs in via `POST /api/users/login` to get `User.Id` (login uses `UserService.LoginAsync`).
- Admin creates/updates food via protected endpoints:
  - `POST /api/foods` requires `CreateFoodDto.UserId` (UserService verifies `Role == "Admin"`).
  - `PATCH /api/foods/{id}` and `PATCH /api/foods/{id}/availability` require a `userId` query parameter to verify admin role.
  - Admin order management endpoints (`GET /api/orders`, `PATCH /api/orders/{id}/status`, `POST /api/orders/{id}/cancel`) also verify admin role.

---
## 2) Edge-case handling (where in code / how handled)

- Duplicate email/phone: `UserService.SignupAsync` checks `IUserRepository.GetByEmailOrPhoneAsync` and throws a friendly error (controller returns 400).
- Invalid or expired referral code: `ReferralService.IsValidAsync` used in signup; invalid referral throws error.
- Invalid or expired OTP: `UserService.VerifyAsync` compares `OtpCode` and `OtpExpiry` and returns false on failure.
- Invalid login credentials: `UserService.LoginAsync` returns `null` -> controller returns 401.
- Food becomes unavailable after added to cart: handled at order creation in `OrderService.CreateAsync` by re-validating `Food.IsAvailable` for each item; returns 400 if any item unavailable.
- Customer cancels order: `OrderService.CancelByCustomerAsync` allows cancellation when order status is `Pending` or `Confirmed` only; otherwise returns false.
- Admin cancels order: `OrderService.CancelAsync` forces status to `Cancelled`.
- Payment not completed: Ticket specified this can be assumed; we mark order creation and leave payment handling to future work (order stays in `Pending` until payment/confirmation transition).
- Concurrency / race: current implementation checks availability at order creation time; to avoid race conditions in high-concurrency scenarios you would add optimistic concurrency or a reservation/locking step (see Scalability section).

Exception handling
- Services raise exceptions for invalid input or business failures; controllers catch and translate to HTTP 4xx/5xx responses. Repositories use EF Core async methods.

---
## 3) Assumptions

- No authentication system required per ticket: we use minimal role verification by checking `User.Role` using a `userId` passed in requests. Admin endpoints require that `userId` corresponds to an `Admin` role.
- Passwords: per your instruction passwords are stored in plain text. This is explicitly insecure — noted in README.
- OTP delivery: actual email/SMS sending is out of scope. For demo the OTP is returned in the signup response.
- Admin creation: signup default role was originally `Customer`; you updated signup to create Admin accounts during testing. The code uses the stored `Role` to authorize admin actions.
- Database: single SQLite instance is used for simplicity.
- No distributed state: all state is in the relational DB; no external cache or queue by default.

---
## 4) Scalability & evolution (100 → 10,000+ users)

The current implementation is intentionally simple. To support growth, consider the following changes:

- Database scaling
  - Move from SQLite to a server RDBMS (Postgres / SQL Server) for concurrency and scale.
  - Add proper indexes on frequently queried columns (User.Email, Food.IsAvailable, Order.UserId, Order.Status).
  - Use read replicas for read-heavy endpoints (food browsing).

- Caching & pagination
  - Cache read-only data (food catalog) in Redis with TTL; invalidate on admin updates.
  - Add pagination for list endpoints (foods, orders).

- Concurrency & ordering
  - Implement optimistic concurrency tokens or database-level transactions when creating orders to avoid double‑booking inventory.
  - Implement a reservation step (place a hold on items) and background task to release holds if payment is not completed.

- Asynchronous processing
  - Use a message queue (RabbitMQ, Azure Service Bus) for long-running tasks: payment processing, notifications, assignment to delivery.
  - Move order status updates (e.g., Confirmed → Preparing) driven by background workers.

- Horizontal scale & stateless services
  - Make API layer stateless; rely on shared DB & caches. Use container orchestration (Kubernetes) and autoscaling.

- Observability
  - Add metrics (Prometheus), request tracing (OpenTelemetry), and structured logs.

- Security & identity
  - Replace plain-text password storage with ASP.NET Core Identity or a secure hasher and add an authentication system (JWT or cookie-based) for real role enforcement.

---
## 5) Where to find code that implements the flows

- Controllers: `ChuksKitchen/Controllers/*` (UsersController, FoodsController, CartsController, OrdersController)
- Services: `ChuksKitchen.Business/Services/*` (UserService, FoodService, CartService, OrderService)
- Repositories: `ChuksKitchen.Business/Repositories/*` and interfaces in `IRepositories`
- Entities & DbContext: `ChuksKitchen.Data/Entity/*`, `ChuksKitchen.Data/DataContext/ApplicationDbContext.cs`
- Diagrams: `diagrams/*.puml` (render to PNG with PlantUML)
