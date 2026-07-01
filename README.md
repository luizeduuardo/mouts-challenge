# Ambev Developer Evaluation — Sales API

A .NET 8 Web API for managing sales and sale items, built with Clean Architecture (Domain / Application / ORM / IoC / WebApi), MediatR, FluentValidation, AutoMapper, EF Core (PostgreSQL) and JWT authentication.

## Requirements

Pick **one** of the two setups below.

| | Docker-only | Local (.NET SDK) |
|---|---|---|
| Needs .NET 8 SDK installed | No | Yes |
| Needs Docker Desktop | Yes | Yes (for the database only) |
| Best for | Just trying the API | Developing / debugging in an IDE |

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (with Compose v2, bundled by default)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) — only needed for the "Local" setup or to run the test suite

No other tools are required. The database schema is created automatically (EF Core migrations run on startup), so there is no manual migration or seeding step.

## 1. Configure

There is nothing to configure for a first run — sensible defaults for local/Docker use already ship in the repo:

- `src/Ambev.DeveloperEvaluation.WebApi/appsettings.json` has a default PostgreSQL connection string pointing at `localhost:5432` with the same credentials used by `docker-compose.yml` (`developer` / `ev@luAt10n` / database `developer_evaluation`).
- `docker-compose.yml` passes a container-to-container connection string to the API via the `ConnectionStrings__DefaultConnection` environment variable, so the two never get out of sync.
- A JWT signing key ships in `appsettings.json` for evaluation purposes. **Do not reuse it for a real deployment** — replace `Jwt:SecretKey` (or set the `Jwt__SecretKey` environment variable) with your own secret first.

If you need to point at a different database (e.g. a local PostgreSQL install instead of Docker), edit `ConnectionStrings:DefaultConnection` in `appsettings.Development.json`, or set the `ConnectionStrings__DefaultConnection` environment variable — no code changes needed.

## 2. Run

### Option A — Everything in Docker (recommended for a quick try)

```bash
docker compose up -d --build
```

This starts PostgreSQL and the API. The API container waits for the database to report healthy before starting, and applies EF Core migrations automatically on boot.

- API base URL: `http://localhost:8085`
- Swagger UI: `http://localhost:8085/swagger`
- Health check: `http://localhost:8085/health`

Stop everything with:

```bash
docker compose down
```

To wipe the database too (start fresh):

```bash
docker compose down -v
```

> The compose file also defines MongoDB and Redis containers. They are scaffolding for future features and are **not** used by the API yet — you can ignore them.

### Option B — API on your machine, database in Docker

Start only the database:

```bash
docker compose up -d ambev.developerevaluation.database
```

Run the API with the .NET SDK:

```bash
dotnet run --project src/Ambev.DeveloperEvaluation.WebApi/Ambev.DeveloperEvaluation.WebApi.csproj
```

- API base URL: `http://localhost:5119`
- Swagger UI: `http://localhost:5119/swagger`

This mode is better for debugging (breakpoints, hot reload with `dotnet watch run`) since the API isn't inside a container.

## 3. Try it out

All endpoints are documented and callable from Swagger UI.

Create a User and authenticate before trying to manage Sales.

Passwords require 8+ characters with at least one uppercase letter, one lowercase letter, one digit and one special character (e.g. `Password123!`).

## 4. Test

Only the **Unit** test project currently has tests (Domain + Application layers, no database needed).

Run all tests:

```bash
dotnet test Ambev.DeveloperEvaluation.sln
```

Run just the unit tests:

```bash
dotnet test tests/Ambev.DeveloperEvaluation.Unit
```

## Troubleshooting

- **Port already in use**: if `8085` or `5432` are taken on your machine, change the left-hand side of the port mapping in `docker-compose.yml` (e.g. `"18085:8080"`) and/or `ConnectionStrings:DefaultConnection` accordingly.
- **`docker compose up` succeeds but the API container exits immediately**: check `docker compose logs ambev.developerevaluation.webapi`. The most common cause is a database that isn't reachable yet — the `depends_on: condition: service_healthy` clause should prevent this, but if you changed the Postgres port/credentials, make sure `ConnectionStrings__DefaultConnection` in `docker-compose.yml` still matches.
- **401 Unauthorized on `/api/sales`**: this endpoint requires a `Bearer` token from `POST /api/auth`; anonymous requests are rejected by design.
