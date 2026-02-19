# Chuks Kitchen - Backend

This repository contains the backend implementation for Chuks Kitchen (ASP.NET 10). It includes working APIs for Users, Foods, Carts and Orders plus admin flows.

Contents
- `ChuksKitchen/` - main web project
- `ChuksKitchen.Business/` - business logic, services, DTOs, repositories
- `ChuksKitchen.Data/` - EF Core entities and DbContext
- `diagrams/` - PlantUML source files for flow and data model diagrams

Quick start
1. Build
   ```bash
   dotnet build
   ```

2. (If you changed entities) Apply EF Core migrations
   - Install tool if required: `dotnet tool install --global dotnet-ef`
   - Add migration and update DB (from solution root):
     ```bash
     dotnet ef migrations add Init --project ChuksKitchen.Data --startup-project ChuksKitchen
     dotnet ef database update --project ChuksKitchen.Data --startup-project ChuksKitchen
     ```
   The project uses SQLite (see `ChuksKitchen/appsettings.json` for `DefaultConnection`).

3. Run
   ```bash
   dotnet run --project ChuksKitchen
   ```
   Swagger UI is available at `https://localhost:{PORT}/swagger`.

API endpoints (summary)
- User
  - `POST /api/users/signup` - Sign up (email or phone, password + confirmPassword, optional referral)
  - `POST /api/users/verify` - Verify OTP
  - `POST /api/users/login` - Login (emailOrPhone + password)

- Food
  - `GET /api/foods` - List available foods (customer view)
  - `GET /api/foods/admin?userId={GUID}` - Admin list all foods (must be admin user)
  - `POST /api/foods` - Create food (body must include `userId` of an Admin)
  - `PATCH /api/foods/{id}` - Update food (admin; pass `userId` query)
  - `PATCH /api/foods/{id}/availability?userId={GUID}` - Set availability (admin)

- Cart
  - `POST /api/carts/add` - Add item to cart (body: `userId`, `foodId`, `quantity`)
  - `GET /api/carts/{userId}` - View cart
  - `POST /api/carts/clear/{userId}` - Clear cart

- Order
  - `POST /api/orders` - Create order from items (body: `userId`, `items`)
  - `GET /api/orders/{id}` - Get order details
  - `GET /api/orders?userId={GUID}` - Admin: list orders (admin check via header previously; use admin endpoints)
  - `PATCH /api/orders/{id}/status` - Update order status (admin)
  - `POST /api/orders/{id}/cancel` - Admin cancel
  - `POST /api/orders/{id}/cancel/customer` - Customer cancel allowed when Pending or Confirmed

Notes, conventions and caveats
- Admin checks: endpoints that require admin now verify the acting user's role by `userId` provided in the request (either in DTO or query). Do not pass `X-Admin` header for production — use `userId`.
- Passwords: per your instruction, passwords are stored in plain text in this demo implementation. This is insecure and should be replaced before production.
- OTP: a random 4-digit OTP is generated at signup and stored on the user record. For testing the OTP is returned in signup response.
- Enums (OrderStatus) are serialized as strings in JSON responses (e.g., `Pending`, `Preparing`).

How to get created userId
- Signup currently returns the created user object (OTP) but if you need the `Id` from signup I can modify the controller to include it. Alternatively inspect the SQLite DB (`DefaultConnection`) to query the `Users` table.

Diagrams and data model
- PlantUML sources are in `diagrams/` and can be rendered to PNG. See `diagrams/README.md` for commands to generate PNG files.

Deliverables included
- Working API (this repo)
- PlantUML sources for required flow diagrams (in `diagrams/`)
- Data model source (PlantUML ER) in `diagrams/`
- README with run & API instructions (this file)

If you want I will now render the PlantUML files into PNG and place them under `diagrams/png/` — I cannot render PNGs inside this environment, but I provide exact commands to run locally or on CI to generate them.

Contact
- If any changes are needed in endpoints, I can update code and rerun the build.

GitHub Copilot
