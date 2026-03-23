# Clean Architecture .NET API Template

A .NET API template following Clean Architecture principles, designed so you **don’t waste time bootstrapping** a new project and can go straight to implementing **business logic**.

---

## Why this template exists

This template is meant to remove repetitive setup work so you can **start implementing business logic immediately** with a clean, maintainable architecture.

---

## Goals of this template

- Clear separation of concerns (Domain / Application / Infrastructure / Presentation)
- Easy to start locally (Docker Compose or `dotnet run`)
- A predictable place to add features, use cases, and integrations
- Keep business rules independent from frameworks and external services

---

## Solution layout

This repository contains the following projects (high-level):

- `CleanArchTemplate.Domain`
- `CleanArchTemplate.Application`
- `CleanArchTemplate.Infrastructure`
- `CleanArchTemplate.SharedKernel`
- `CleanArchTemplate.Presentation.Api` (**startup project**)

And the solution file:

- `CleanArchTemplate.sln`

---

## Project structure explained (where to put what)

### `CleanArchTemplate.Domain`
**What it is:** The core of the system.  
**What goes here:**
- Entities / Aggregates
- Value Objects
- Domain Events
- Domain services (pure business logic)
- Business rules/invariants

**Notes:**
- Avoid dependencies on ASP.NET Core, EF Core, or any external libraries that “talk to the outside world”.
- This project should be the most stable over time.

---

### `CleanArchTemplate.Application`
**What it is:** Application use-cases and orchestration.  
**What goes here:**
- Use cases / application services (commands/queries - CQRS)
- Validation and business workflows that coordinate domain objects

---

### `CleanArchTemplate.Infrastructure`
**What it is:** Implementation details and integrations.  
**What goes here:**
- EF Core `DbContext`, mappings, configurations
- External service clients (HTTP clients, message brokers, file storage, etc.)

**Notes:**
- This layer depends on `Application` and `Domain`.
- This is where “real world” concerns live.

---

### `CleanArchTemplate.Presentation.Api` (Startup Project)
**What it is:** The HTTP API boundary (ASP.NET Core).  
**What goes here:**
- Controllers
- API configuration (DI wiring, middleware, Swagger, auth, etc.)

**Notes:**
- This project is the entry point of the application and is the **startup project**.

---

### `CleanArchTemplate.SharedKernel`
**What it is:** Cross-cutting/shared building blocks used across layers. 

---

## Running the project

### Option A — Run with Docker Compose (recommended for local DB)

This repo includes a `compose.yaml` that starts:
- API container (exposes **port 8080**)
- PostgreSQL container (exposes **port 5432**)

**Start:**
```bash
docker compose up --build
```

**API URL:**
- http://localhost:8080

**Database (from `compose.yaml`):**

> Note: inside Docker networking, the API uses `Host=db` in its connection string.

**Stop:**
```bash
docker compose down
```

**Stop + remove volumes (deletes local DB data):**
```bash
docker compose down -v
```

---

### Option B — Run with `dotnet run`

**Prerequisites:**
- .NET SDK installed
- A running PostgreSQL instance (you can still use Docker only for the DB if you want)

**1) Start only the database via Docker (optional but common):**
```bash
docker compose up -d db
```

**2) Run the API (startup project):**
From the repository root:
```bash
dotnet run --project CleanArchTemplate.Presentation.Api
```

If your API needs environment variables / connection strings, set them in your shell or user secrets as appropriate.

---

## Entity Framework Core migrations

Migrations live in **Infrastructure**, but they must run using the API as the **startup project**.

### Add a migration
```bash
dotnet ef migrations add <NameOfMigration> --startup-project CleanArchTemplate.Presentation.Api --project CleanArchTemplate.Infrastructure
```

### Apply migrations (update database)
```bash
dotnet ef database update --startup-project CleanArchTemplate.Presentation.Api --project CleanArchTemplate.Infrastructure
```

---

## Typical workflow for adding a new feature

1. **Domain:** create/update entities, value objects
2. **Application:** add a use case (command/query), interfaces needed
3. **Infrastructure:** implement persistence or integrations (EF Core, external APIs)
4. **Presentation:** expose the use case via HTTP endpoint/controller and wire DI

---

## Notes / conventions

- Keep business logic in **Domain** and **Application**, not in controllers.
- Infrastructure should be “pluggable”: swap Postgres, email provider, etc. with minimal changes outside Infrastructure.
- Presentation is a thin layer: it translates HTTP ↔ application use cases.
