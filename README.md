# 🛒 E-Commerce API

An **ASP.NET Core Web API** for an e-commerce application, built with **.NET 8**, **Entity Framework Core**, **SQL Server**, **Redis**, **ASP.NET Core Identity**, **JWT Authentication**, and **Stripe**.

The API provides the backend functionality for an online store, including product catalog browsing, filtering and pagination, Redis-backed shopping baskets, customer registration and authentication, order management, delivery methods, and online payment processing through Stripe.

The solution follows **Onion Architecture**, separating the domain model, application use cases, infrastructure integrations, and HTTP API into four dedicated projects.

## ✨ Project Highlights

- 🏗️ **Onion Architecture:** Separates Domain, Application, Infrastructure, and API responsibilities.
- 🛍️ **Product Catalog:** Retrieves products, brands, and types with filtering, searching, sorting, and pagination.
- 🔎 **Specification Pattern:** Encapsulates query criteria, includes, ordering, and pagination logic.
- 🧺 **Redis Shopping Basket:** Stores and retrieves customer baskets in Redis with a configurable time-to-live (7 days by default).
- ⚡ **Product Response Caching:** Uses a custom ASP.NET Core Action Filter to cache the product listing response in Redis.
- 👤 **Customer Identity:** Supports registration, login, email availability checks, current-user information, and customer address management.
- 🔐 **JWT Tokens:** Generates bearer tokens containing user identity and role claims.
- 📦 **Order Management:** Creates orders from customer baskets and provides order history and order details.
- 🚚 **Delivery Methods:** Retrieves available delivery options and includes delivery costs in order totals.
- 💳 **Stripe Integration:** Creates and updates Stripe PaymentIntents without sending raw card details through the API.
- 🔔 **Stripe Webhooks:** Handles successful and failed PaymentIntent events and updates the matching order status.
- 🗄️ **EF Core Code-First:** Uses SQL Server, entity configurations, and migrations for the catalog, order, and Identity databases.
- 🧩 **Reusable Infrastructure:** Uses Generic Repository, Unit of Work, Dependency Injection, AutoMapper, and a shared Result/ProblemDetails response approach.

## 🧭 System Overview

The API is organized around five main functional areas:

| Module | Description |
| --- | --- |
| **Products** | Provides product catalog data, including brands and types, with query-based filtering, sorting, searching, and pagination. |
| **Basket** | Creates, retrieves, updates, and deletes customer baskets stored in Redis. |
| **Authentication & Identity** | Registers customers, authenticates credentials, issues JWT tokens, and manages customer address information. |
| **Orders** | Creates customer orders, retrieves order history and individual orders, and exposes available delivery methods. |
| **Payments** | Creates or updates Stripe PaymentIntents and processes Stripe webhook notifications to update order payment status. |

## 🏗️ Architecture

The solution is divided into four projects. Each project has a specific responsibility and depends on the appropriate inner layers.

### Domain Layer — `E_Commerce.Domain`

Contains the core business entities and abstractions. It defines the product, basket, and order domain models, together with contracts for repositories, specifications, the Unit of Work, and data seeding.

### Application Layer — `E_Commerce.Application`

Contains application use cases and business services. It defines service contracts, DTOs, AutoMapper profiles, specifications, payment abstractions, common result types, and application-level operations.

### Infrastructure Layer — `E_Commerce.Infrastructure`

Implements the external and persistence concerns of the application, including:

- SQL Server database contexts and Entity Framework Core configurations.
- EF Core migrations for the store and Identity databases.
- Generic Repository and Unit of Work implementations.
- Redis basket and cache repositories.
- ASP.NET Core Identity services and JWT token generation.
- Stripe PaymentIntent integration.
- Catalog and Identity data seeders.

### API Layer — `E_Commerce.API`

Exposes the application through REST endpoints. It contains the API controllers, the Redis caching Action Filter, application startup and dependency-injection configuration, static product-image hosting, and development Swagger configuration.

### Application Dependency Flow

```text
                     Client / Postman
                            │
                            ▼
                     E_Commerce.API
                    (HTTP Controllers)
                            │
                            ▼
                  E_Commerce.Application
               (Services, DTOs, Specifications)
                            │
                            ▼
                    E_Commerce.Domain
               (Entities and Abstractions)
                            ▲
                            │ Implements Contracts
                            │
                 E_Commerce.Infrastructure
             (EF Core, SQL Server, Redis, Stripe,
                   Identity and Repositories)
                            │
             ┌──────────────┼───────────────┐
             ▼              ▼               ▼
         SQL Server       Redis           Stripe
```

## 🧩 Main Modules

### 🛍️ Product Catalog

The catalog is based on three main entities:

| Entity | Responsibility |
| --- | --- |
| `Product` | Stores product information such as name, description, picture path, price, brand, and type. |
| `ProductBrand` | Represents a product brand. |
| `ProductType` | Represents a product category/type. |

A product is related to one brand and one type. The product listing endpoint supports the following query parameters:

| Parameter | Description |
| --- | --- |
| `BrandId` | Filters products by brand. |
| `TypeId` | Filters products by type. |
| `SearchValue` | Searches product names. |
| `Sort` | Sorts by name or price. |
| `PageIndex` | Requested page number; defaults to `1`. |
| `PageSize` | Number of items per page; defaults to `5` and is limited to a maximum of `10`. |

Supported `Sort` values are `None`, `NameAscending`, `NameDescending`, `PriceAscending`, and `PriceDescending`.

The listing response uses `PaginationResult<ProductDto>`, which contains `PageIndex`, `PageSize`, `Count`, and `Data`. Product images are served from the API's `/Files` path, with their absolute URL built using the configured `UrlSettings:BaseUrl`.

### 🧺 Shopping Basket & Redis

The basket is represented by `CustomerBasket` and its collection of `BasketItem` objects. Basket data is stored in Redis through `IBasketRepository` and serialized as JSON.

- A basket is retrieved using its basket ID.
- A basket can be created or updated through the same endpoint.
- A basket can be deleted when it is no longer needed.
- Basket entries are kept for **7 days by default**.
- The basket can store its selected delivery method and Stripe payment information, including `PaymentIntentId` and `ClientSecret`.

The product listing endpoint also uses the custom `[RedisCache(90)]` Action Filter, which caches successful responses for **90 seconds**.

### 👤 Authentication & Identity

Customer identity is implemented using **ASP.NET Core Identity** with a separate `StoreIdentityDbContext` and SQL Server database.

The authentication service supports:

- Customer registration.
- Login using email and password.
- Email existence checks.
- Retrieving the current authenticated user.
- Retrieving and updating the authenticated user's address.

Successful registration and login return a `UserDto` containing the user's email, display name, and JWT token. The JWT includes identity claims and role claims and uses the configured issuer, audience, signing key, and expiration period.

### 📦 Order Management

The order module converts a customer's Redis basket into a persisted order. It uses the current product catalog prices when building order items and stores product information in the order item snapshot.

Main order entities:

| Entity | Responsibility |
| --- | --- |
| `Order` | Stores buyer email, shipping address, order date, status, subtotal, delivery method, and PaymentIntent ID. |
| `OrderItem` | Stores the ordered product snapshot, unit price, and quantity. |
| `ProductItemOrdered` | Keeps the product ID, name, and picture URL as part of the order item. |
| `OrderAddress` | Stores the shipping address associated with the order. |
| `DeliveryMethod` | Stores the delivery option, description, delivery time, and cost. |

The shipping address and ordered product details are retained with the order so the order record is not dependent on later changes to the customer's address or product catalog entry.

The available order statuses are:

- `Pending`
- `PaymentReceived`
- `PaymentFailed`

### 💳 Stripe Payment Processing

The payment module uses **Stripe PaymentIntents** to prepare and track card payments.

1. The client requests a PaymentIntent for a basket.
2. The API retrieves the basket and validates its items and delivery method.
3. The service reloads current product prices from the catalog and recalculates the delivery cost and payment amount on the server.
4. The API creates a PaymentIntent when the basket does not already have one, or updates the existing PaymentIntent when it does.
5. The PaymentIntent ID and Client Secret are saved with the basket and returned to the client.
6. The client uses Stripe's client-side SDK to complete the payment directly with Stripe. Raw card details are not sent to this API.
7. After the order is created, Stripe sends a webhook event to the API. The API verifies the `Stripe-Signature` and updates the matching order to `PaymentReceived` or `PaymentFailed`.

The API currently handles these Stripe event types:

- `payment_intent.succeeded`
- `payment_intent.payment_failed`

## 🔌 API Endpoints

All routes are based on the controller route `api/[controller]`. The table below reflects the routes and authorization attributes currently present in the repository's `master` branch.

### Products

| Method | Endpoint | Description |
| --- | --- | --- |
| `GET` | `/api/Products` | Retrieves products with optional filtering, sorting, searching, and pagination. |
| `GET` | `/api/Products/{id}` | Retrieves a product by ID. Marked with `[Authorize]` in the current controller. |
| `GET` | `/api/Products/types` | Retrieves all product types. |
| `GET` | `/api/Products/brands` | Retrieves all product brands. |

### Basket

| Method | Endpoint | Description |
| --- | --- | --- |
| `GET` | `/api/Basket/{id}` | Retrieves a basket by its ID. |
| `POST` | `/api/Basket` | Creates or updates a basket. |
| `DELETE` | `/api/Basket/{id}` | Deletes a basket by its ID. |

### Authentication

| Method | Endpoint | Description |
| --- | --- | --- |
| `POST` | `/api/Authentication/Login` | Authenticates a customer and returns a JWT token. |
| `POST` | `/api/Authentication/register` | Registers a customer and returns a JWT token. |
| `GET` | `/api/Authentication/emailexists?email={email}` | Checks whether an email address already exists. |
| `GET` | `/api/Authentication/CurrentUser` | Retrieves the current authenticated user. `[Authorize]` |
| `GET` | `/api/Authentication/Address` | Retrieves the current user's address. `[Authorize]` |
| `PUT` | `/api/Authentication/Address` | Creates or updates the current user's address. `[Authorize]` |

### Orders

| Method | Endpoint | Description |
| --- | --- | --- |
| `POST` | `/api/Orders` | Creates an order from a basket and shipping address. `[Authorize]` |
| `GET` | `/api/Orders` | Retrieves the authenticated user's orders. `[Authorize]` |
| `GET` | `/api/Orders/{id}` | Retrieves one order belonging to the authenticated user. `[Authorize]` |
| `GET` | `/api/Orders/DeliverMethods` | Retrieves available delivery methods. `[AllowAnonymous]` |

### Payments

| Method | Endpoint | Description |
| --- | --- | --- |
| `POST` | `/api/Payments/{basketId}` | Creates or updates the Stripe PaymentIntent for a basket. `[Authorize]` |
| `POST` | `/api/Payments/webhook` | Receives Stripe webhook events and updates the matching order's payment status. Requires a valid Stripe signature. |

> **Authentication note:** The controllers mark certain endpoints with `[Authorize]`, and JWT bearer authentication is registered in the infrastructure setup.

> **Basket security note:** The current Basket endpoints do not have `[Authorize]` attributes, and basket responses can include the Stripe `ClientSecret`. Do not expose basket IDs or payment secrets publicly. Restrict basket access to its authenticated owner before using this implementation in a production environment.

## 🗂️ Project Structure

```text
E_Commerce-API/
│
├── E_Commerce..sln
│
├── E_Commerce.API/
│   ├── Attributes/
│   │   └── RedisCacheAttribute.cs
│   ├── Controllers/
│   │   ├── ApiBaseController.cs
│   │   ├── AuthenticationController.cs
│   │   ├── BasketController.cs
│   │   ├── OrdersController.cs
│   │   ├── PaymentsController.cs
│   │   └── ProductsController.cs
│   ├── Extentions/
│   │   └── WebAppExtentions.cs
│   ├── Files/
│   │   └── Images/Products/
│   ├── Properties/
│   │   └── launchSettings.json
│   ├── Program.cs
│   └── appsettings*.json
│
├── E_Commerce.Application/
│   ├── Common/
│   ├── Contracts/
│   ├── DTOs/
│   │   ├── Basketd/
│   │   ├── Identity/
│   │   ├── Orders/
│   │   └── Products/
│   ├── Profiles/
│   ├── Services/
│   └── Specifications/
│
├── E_Commerce.Domain/
│   ├── Common/
│   ├── Contracts/
│   └── Entities/
│       ├── Baskets/
│       ├── Orders/
│       └── Products/
│
└── E_Commerce.Infrastructure/
    ├── Data/
    │   ├── Configurations/
    │   ├── Migrations/
    │   └── StoreDbContext.cs
    ├── DataSeeding/
    ├── Identity/
    │   ├── Data/Migrations/
    │   ├── Entities/
    │   └── Services/
    ├── Payments/
    ├── Repositories/
    └── Specifications/
```

### 📁 Folder Responsibilities

| Project / Folder | Responsibility |
| --- | --- |
| `E_Commerce.API/Controllers` | Defines the HTTP endpoints for products, baskets, authentication, orders, and payments. |
| `E_Commerce.API/Attributes` | Contains the custom Redis response-caching Action Filter. |
| `E_Commerce.API/Extentions` | Contains application startup extensions such as database migration and seeding orchestration. |
| `E_Commerce.Application/Contracts` | Defines abstractions for application services, Identity, tokens, and payment integrations. |
| `E_Commerce.Application/DTOs` | Defines request and response models used by the API. |
| `E_Commerce.Application/Profiles` | Contains AutoMapper mappings and picture URL resolvers. |
| `E_Commerce.Application/Services` | Implements product, basket, authentication, order, cache, and payment use cases. |
| `E_Commerce.Application/Specifications` | Defines reusable query criteria, includes, ordering, and pagination rules. |
| `E_Commerce.Domain/Entities` | Contains basket, product, and order domain entities. |
| `E_Commerce.Domain/Contracts` | Defines repository, specification, Unit of Work, cache, and data-seeder abstractions. |
| `E_Commerce.Infrastructure/Data` | Contains the store DbContext, EF Core configurations, and store migrations. |
| `E_Commerce.Infrastructure/Identity` | Contains the Identity DbContext, user/address entities, Identity migrations, and Identity services. |
| `E_Commerce.Infrastructure/Repositories` | Implements the Generic Repository, Unit of Work, Redis basket repository, and Redis cache repository. |
| `E_Commerce.Infrastructure/Payments` | Implements the Stripe payment gateway. |

## 🛠️ Technologies & Tools

| Technology | Purpose |
| --- | --- |
| **C# / .NET 8** | Main development platform. |
| **ASP.NET Core Web API** | Builds the REST API and HTTP controllers. |
| **Entity Framework Core 8** | Object-relational mapping and Code-First migrations. |
| **SQL Server** | Stores product catalog, orders, and Identity data. |
| **ASP.NET Core Identity** | Manages customer accounts and roles. |
| **JWT Bearer Authentication** | Issues and validates access tokens for authenticated endpoints. |
| **Redis / StackExchange.Redis** | Stores customer baskets and cached product responses. |
| **Stripe.net** | Creates and updates PaymentIntents and integrates with Stripe events. |
| **AutoMapper** | Maps entities to API DTOs and back. |
| **Generic Repository & Unit of Work** | Encapsulates data access operations. |
| **Specification Pattern** | Encapsulates reusable filtering, sorting, includes, and pagination. |
| **Postman** | Organizes and tests the API endpoints. |

## 🚀 Getting Started

### Prerequisites

Before running the API locally, make sure you have:

- **.NET 8 SDK**
- **SQL Server** instance
- **Redis** server
- **Visual Studio** or another .NET-compatible development environment
- A **Stripe account** with test API keys for payment testing
- **Stripe CLI** for local webhook testing

### 1. Clone the Repository

```bash
git clone https://github.com/Abdelrahmankishk/E_Commerce-API.git
cd E_Commerce-API
```

### 2. Restore NuGet Packages

```bash
dotnet restore E_Commerce..sln
```

### 3. Configure Application Settings

The API reads configuration from `appsettings.json`, environment variables, and ASP.NET Core User Secrets. Configure the following values for your environment:

| Setting | Description |
| --- | --- |
| `ConnectionStrings:DefaultConnection` | SQL Server connection string for the store database. |
| `ConnectionStrings:IdentityConnection` | SQL Server connection string for the Identity database. |
| `ConnectionStrings:RedisConnection` | Redis connection string; the current default is `localhost`. |
| `UrlSettings:BaseUrl` | Base URL used when generating product and order item picture URLs. |
| `JWT:SecretKey` | Secret key used to sign JWT tokens. Use a strong key of at least 32 characters. |
| `JWT:Issuer` | Expected JWT issuer. |
| `JWT:Audience` | Expected JWT audience. |
| `JWT:ExpirationMinutes` | JWT expiration time in minutes. |
| `Stripe:SecretKey` | Stripe secret API key (use a test key for local development). |
| `Stripe:DefaultCurrency` | Currency used when creating PaymentIntents, for example `USD`. |
| `Stripe:WebhookSecret` | Webhook signing secret provided by Stripe CLI or the Stripe dashboard. |

For local development, use **User Secrets** instead of storing credentials in `appsettings.json`:

```bash
dotnet user-secrets init --project E_Commerce.API/E_Commerce.API.csproj

dotnet user-secrets set "JWT:SecretKey" "YOUR_STRONG_SECRET_KEY_AT_LEAST_32_CHARACTERS" --project E_Commerce.API/E_Commerce.API.csproj
dotnet user-secrets set "Stripe:SecretKey" "sk_test_YOUR_STRIPE_SECRET_KEY" --project E_Commerce.API/E_Commerce.API.csproj
dotnet user-secrets set "Stripe:DefaultCurrency" "USD" --project E_Commerce.API/E_Commerce.API.csproj
dotnet user-secrets set "Stripe:WebhookSecret" "whsec_YOUR_WEBHOOK_SECRET" --project E_Commerce.API/E_Commerce.API.csproj
```

Also configure the two SQL Server connection strings if your local database instance differs from the defaults in `appsettings.json`. The current launch settings use `https://localhost:7273` and `http://localhost:5041`; set `UrlSettings:BaseUrl` to the URL you use for the API so generated image links point to the correct host.

### 4. Database Migrations & Seeding

The application calls `SeedAndMigrateDataAsync()` during startup. The store and Identity seeders check for pending EF Core migrations and apply them automatically.

**Catalog seed data must be supplied separately:** `CatalogDataSeeder` looks for the following JSON files in a `DataSeed` directory under the application's runtime output directory:

```text
DataSeed/
├── brands.json
├── types.json
├── products.json
└── delivery.json
```

These JSON files are **not included in the current tracked repository**. Add the required seed files and configure them to be copied to the API output directory before expecting the catalog and delivery method tables to be populated. Without them, the database schema can be migrated but the catalog and delivery data will remain empty.


## 🧪 Postman Collection

The exported Postman collection contains the API requests and their descriptions, organized by module.

### 📥 Import the Collection

[**Download the E-Commerce API Postman Collection**](https://drive.google.com/file/d/15rwxf9yIF4v5O9X41Tb7nIJ1HX0gyUeH/view?usp=sharing)

1. Open the Google Drive link and download the exported collection JSON file.
2. Open **Postman** and select **Import**.
3. Select the downloaded JSON file (or drag it into the Import window).
4. Configure the API base URL and Bearer token for authenticated requests.
5. Run the requests in the collection to explore the API modules.

### Local Stripe Webhook Testing

Stripe cannot directly send webhook requests to a local `localhost` address. Use Stripe CLI to forward signed test events to the local API:

```bash
stripe login
stripe listen --forward-to localhost:5041/api/Payments/webhook
```

The CLI outputs a temporary `whsec_...` signing secret. Configure it as `Stripe:WebhookSecret` in User Secrets, then trigger a test event in another terminal:

```bash
stripe trigger payment_intent.succeeded
```

The webhook endpoint verifies the `Stripe-Signature` header before processing supported payment events.
