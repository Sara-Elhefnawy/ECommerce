# ECommerce

An e-commerce backend API built with **ASP.NET Core Minimal APIs**, clean architecture and CQRS. It covers catalog management, carts, orders and Stripe payments.

## Features

- **Catalog**: brands, categories and products, with paginated and filterable product endpoints (Ardalis Specification).
- **Cart**: stored in HybridCache (in-memory plus Redis), with expiration policies configured centrally in `appsettings`.
- **Orders and payments**: Stripe PaymentIntents with webhook-driven status changes.
  - `Pending` → `Processing` on `payment_intent.succeeded`
  - `Pending` → `PaymentFailed` on `payment_intent.payment_failed`
  - A retry with a new card after a decline can still mark the order paid.
- **Accounts**: email verification with one-time codes; password reset with hashed tokens (SHA256) stored in cache.
- **Images**: uploaded to Cloudinary.
- **Unified responses**: every endpoint returns an `ApiResponse<T>` envelope through `ResultExtensions`.
- **API versioning** with `ApiVersionSet`, and Swagger documentation.
- **Audit logging** through an `IEndpointFilter` that records the real response status code.
- **Structured logging** with Serilog, writing to PostgreSQL.

## Architecture

```
ECommerce.sln
├── src/
│   ├── ECommerce.API              # Minimal API endpoints, filters, composition root
│   ├── ECommerce.APP              # Commands, queries, handlers, pipeline behaviors
│   ├── ECommerce.Domain           # Entities, value objects, domain rules
│   └── ECommerce.Infrastructure   # EF Core, caching, email, Stripe, Cloudinary
└── test/
    └── ECommerce.ArchTest         # Architecture rules
```

- **Vertical slices and CQRS**: each feature has its own request, handler, validator and endpoint.
- **Custom mediator pipeline**: logging and FluentValidation run as pipeline behaviors.
- **Endpoint auto-registration**: every class implementing `IEndpoint` is discovered by reflection and mapped by `MapEndpoints()`, so new endpoints need no manual wiring.
- **Repository-style abstractions over cache**: handlers depend on intent-named repositories (for example `IResetPasswordRepository`), not on `ICache<T>` directly.
- **Email concerns are split**: `IEmailVerification` handles OTP codes, `IEmailService` sends custom emails, and templates are loaded as files through `IEmailTemplateProvider`, never embedded in code.

## Tech stack

ASP.NET Core Minimal APIs · EF Core · Ardalis.Specification · FluentValidation · HybridCache + Redis · PostgreSQL · SQLite · Serilog · Stripe · Cloudinary · Swagger

## Getting started

### Prerequisites

- .NET SDK (matching the version in the solution)
- Redis (optional locally, since the in-memory cache layer works without it)
- PostgreSQL (for log storage)
- A Stripe account (test keys) and a Cloudinary account

### Configure

Set these with `dotnet user-secrets` or environment variables. Never commit real keys.

```bash
dotnet user-secrets set "Stripe:SecretKey" "<sk_test_...>" --project src/ECommerce.API
dotnet user-secrets set "Stripe:WebhookSecret" "<whsec_...>" --project src/ECommerce.API
dotnet user-secrets set "Cloudinary:Url" "<cloudinary-url>" --project src/ECommerce.API
dotnet user-secrets set "ConnectionStrings:Logging" "<postgres-connection-string>" --project src/ECommerce.API
```

### Run

```bash
git clone <your-repo-url>
cd ECommerce
dotnet run --project src/ECommerce.API
```

Open `/swagger` to explore the API. To receive Stripe webhooks locally:

```bash
stripe listen --forward-to https://localhost:<port>/api/v1/payments/webhook
```

## Testing

```bash
dotnet test
```

- **Architecture tests** (NetArchTest) enforce the layer dependencies.
- **Integration tests** use xUnit, `WebApplicationFactory`, Testcontainers (Redis) and Shouldly.
- **Load tests** use NBomber.
