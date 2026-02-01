# BoricuaCoder Clean Template (Without Dogma)

A pragmatic .NET 10 Clean Architecture starter template. Production-ready, minimal ceremony, easy to extend.

## Architecture

```
src/
  BoricuaCoder.CleanTemplate.Api            -> Presentation (Controllers, DTOs)
  BoricuaCoder.CleanTemplate.Application    -> Use cases (Commands, Queries, Handlers, Validation)
  BoricuaCoder.CleanTemplate.Domain         -> Entities, Value Objects, Domain rules
  BoricuaCoder.CleanTemplate.Infrastructure -> Dapper repositories, DB access, External services
tests/
  BoricuaCoder.CleanTemplate.UnitTests      -> NUnit + Moq
```

**Dependency flow:** Api -> Application -> Domain <- Infrastructure

Domain references nothing. Infrastructure implements the ports defined in Application.

## Tech Stack

| Concern | Choice |
|---|---|
| Framework | .NET 10 / ASP.NET Core |
| API Style | Controllers |
| Persistence | Dapper + PostgreSQL |
| CQRS | Lightweight (no MediatR) |
| Validation | FluentValidation |
| Auth + Swagger | BoricuaCoder.API.CoreSetup |
| Testing | NUnit + Moq |

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL (local or Docker)

### Database Setup

1. Create a PostgreSQL database:
   ```sql
   CREATE DATABASE cleantemplate;
   ```

2. Run the schema script:
   ```bash
   psql -d cleantemplate -f src/BoricuaCoder.CleanTemplate.Infrastructure/Database/schema.sql
   ```

3. Update the connection string in `src/BoricuaCoder.CleanTemplate.Api/appsettings.json`.

### Run

```bash
dotnet run --project src/BoricuaCoder.CleanTemplate.Api
```

### Test

```bash
dotnet test
```

## How to Add a New Feature

1. **Domain** — Create entity in `Domain/YourFeature/YourEntity.cs` with factory method and domain rules.

2. **Application** — Add:
   - `Application/YourFeature/IYourEntityRepository.cs` (port)
   - `Application/YourFeature/DTOs/YourEntityResponse.cs`
   - `Application/YourFeature/Commands/CreateYourEntity/` — Command, Validator, Handler
   - `Application/YourFeature/Queries/GetYourEntityById/` — Query, Handler
   - Register handlers automatically (assembly scanning in `DependencyInjection.cs` handles this).

3. **Infrastructure** — Add:
   - `Infrastructure/YourFeature/YourEntityRepository.cs` (Dapper implementation)
   - SQL schema in `Infrastructure/Database/schema.sql`
   - Register the repository in `Infrastructure/DependencyInjection.cs`

4. **API** — Add `Api/Controllers/YourFeatureController.cs` injecting the handlers.

5. **Tests** — Add tests in `tests/` mirroring the feature structure.

## Auth Configuration

This template uses [BoricuaCoder.API.CoreSetup](https://www.nuget.org/packages/BoricuaCoder.API.CoreSetup) for JWT authentication and Swagger with OAuth2/PKCE.

Configure in `appsettings.json` under the `CoreSetup` section:

```json
{
  "CoreSetup": {
    "Jwt": {
      "Authority": "https://your-idp.com/realms/your-realm",
      "Audience": "your-api-audience",
      "RequireHttpsMetadata": true
    },
    "Swagger": {
      "AuthorizationUrl": "https://your-idp.com/.../auth",
      "TokenUrl": "https://your-idp.com/.../token",
      "Scopes": { "openid": "OpenID", "profile": "Profile" }
    }
  }
}
```

## What We Intentionally Avoided ("Without Dogma")

- **No MediatR** — Handlers are resolved via DI directly. Simple, explicit, debuggable.
- **No extra layers** — No separate "Contracts", "Interfaces", or "Shared" projects. Four layers are enough.
- **No EF Core** — Dapper keeps data access simple and fast. SQL is explicit, not hidden behind LINQ.
- **No generic repository** — Each repository has its own interface with domain-specific methods.
- **No over-abstraction** — No base entity classes, no aggregate root abstractions, no domain events (yet). Add them when you need them.
- **No ceremony** — No marker interfaces for every concept. Just enough structure to keep boundaries clean.

This template is designed to be extended. Future blog posts will layer on DDD patterns, hybrid caching, and more — when they're needed, not before.
