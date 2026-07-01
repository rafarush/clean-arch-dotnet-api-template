# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build

# Run API locally (requires PostgreSQL)
dotnet run --project CleanArchTemplate.Presentation.Api

# Full stack via Docker (API on :8080, PostgreSQL on :5432)
docker compose up --build

# Start only the database
docker compose up -d db
```

### EF Core migrations (Infrastructure project, Api as startup)

```bash
dotnet ef migrations add <Name> \
  --startup-project CleanArchTemplate.Presentation.Api \
  --project CleanArchTemplate.Infrastructure

dotnet ef database update \
  --startup-project CleanArchTemplate.Presentation.Api \
  --project CleanArchTemplate.Infrastructure
```

## Architecture

Five projects with strict dependency flow (inner layers never depend on outer):

| Project | Role | Key dependencies |
|---|---|---|
| `Domain` | Entities, value objects, domain events | None |
| `SharedKernel` | Shared `*Input` / `*Output` DTOs | None |
| `Application` | CQRS use cases, interfaces, validators | Domain, SharedKernel |
| `Infrastructure` | EF Core, repositories, email, OAuth impls | Application, Domain |
| `Presentation.Api` | Controllers, DI wiring, Swagger, JWT middleware | Application |

`Presentation.Api` is the startup project. Migrations live in `Infrastructure` but need `Presentation.Api` as the startup project to resolve DI.

## Key patterns

**CQRS with MediatR**  
Commands implement `ICommand<TResult>`, queries implement `IQuery<TResult>`. The handler is `internal sealed`, co-located in the same file as its command/query record, under `Application/Features/<Domain>/Commands/` or `.../Queries/`. Controllers never inject `IMediator` directly — they use `ICommandSender` / `IQuerySender`.

**Result\<T\>**  
All handlers return `Result<T>`. Use the typed factory methods:
```csharp
Result<T>.Success(value)
Result<T>.NotFound("error message")
Result<T>.Unauthorized("error message")
Result<T>.Conflict("error message")
Result<T>.Validation("error message")
Result<T>.BusinessRule("error message")
Result<T>.InternalError("error message")
```
`BaseController` maps these to HTTP status codes automatically.

**Validation**  
FluentValidation validators are registered automatically via `AddValidatorsFromAssemblyContaining<IApplicationMarker>`. Handlers call `await validator.ValidateAndThrowAsync(input, ct)` at the top.

**DI wiring**  
Each layer registers its own services through extension methods called in `Program.cs`: `AddApi`, `AddEmailService`, `AddApplication`, `AddDatabase`. Add new registrations to the matching extension method in the layer that owns the implementation.

**Authorization**  
JWT Bearer auth with a custom permission-based system (`IPermissionService`, `PermissionAuthorizationHandler`, `PermissionPolicyProvider`). OAuth providers (Google, GitHub) are wired in `Application/Features/Auth`.

## Adding a new feature

1. **Domain** — add/update entity or value object
2. **SharedKernel** — add `*Input` / `*Output` records
3. **Application** — create `*Command.cs` or `*Query.cs` with co-located `internal sealed` handler; add repository/service interfaces if needed; add FluentValidation validator
4. **Infrastructure** — implement any new interfaces; add EF mapping if needed; run a migration
5. **Presentation.Api** — add controller action calling `ICommandSender.SendAsync` / `IQuerySender.SendAsync`; register any new Presentation-layer services in `ApiServiceCollectionExtensions`
