# trading-project-persistence

A microservice in the **dot-trading** platform responsible for persisting all trading data — trades, opportunities, and portfolio snapshots — and exposing that data through a RESTful API for the rest of the system.

---

## Purpose

This service is the single source of truth for historical and real-time trading state within the dot-trading platform. It receives write events from trading bots and AI agents (open/close trade, detected opportunity, portfolio snapshot), persists them to PostgreSQL, and serves read queries for dashboards, analytics, and decision-making services.

It is intentionally focused: no trading logic, no order execution, no signal generation. Its only job is reliable persistence and querying of trading data.

---

## Role in the Microservices Architecture

```
┌─────────────────────┐     ┌──────────────────────────────┐
│   Trading Bot /     │────▶│  trading-project-persistence  │
│   AI Agent          │     │  (this service)               │
└─────────────────────┘     │                              │
                             │  - Stores trades             │
┌─────────────────────┐     │  - Stores opportunities      │
│   Dashboard /       │◀────│  - Stores portfolio snapshots │
│   Analytics Service │     │  - Serves analytics queries   │
└─────────────────────┘     └──────────────┬───────────────┘
                                            │
                                     ┌──────▼──────┐
                                     │  PostgreSQL  │
                                     └─────────────┘
```

In the Kubernetes cluster, this service runs in the `trading-ai` namespace as a ClusterIP service, reachable by other services in the namespace via internal DNS.

---

## Features

- **Trade lifecycle tracking** — log trade opens, closes, take-profit updates, and query by status or symbol
- **Opportunity logging** — record AI-detected trading signals with score, reason, target, and stop-loss
- **Portfolio snapshots** — store point-in-time portfolio state (balance, P&L, open positions count)
- **Analytics API** — pre-built queries for daily/weekly/monthly/yearly P&L, win rate, open positions, and recent closed trades
- **OpenAPI/Swagger UI** — available in Development mode at `/api/docs`

---

## Tech Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 10 / ASP.NET Core |
| Language | C# 12 |
| Database | PostgreSQL (via EF Core 10 + Npgsql) |
| CQRS / Mediator | Cortex.Mediator |
| Object Mapping | AutoMapper 13 |
| Container | Docker (multi-stage build) |
| Orchestration | Kubernetes |

---

## Architecture

The project follows **Clean Architecture** with four layers:

```
TradingProject.Persistence.Api           ← HTTP endpoints, request routing
TradingProject.Persistence.Application  ← CQRS handlers, use cases, abstractions
TradingProject.Persistence.Domain       ← Domain entities (Trade, Opportunity, PortfolioSnapshot)
TradingProject.Persistence.Infrastructure ← EF Core DbContext, DatabaseService, connection config
```

Commands and queries are dispatched through the mediator so controllers never touch the database directly.

---

## API Reference

### Trades — `/api/trades`

| Method | Path | Description |
|---|---|---|
| `GET` | `/api/trades` | List trades. Filters: `limit`, `status`, `symbol` |
| `POST` | `/api/trades` | Create a new trade record |
| `PUT` | `/api/trades/{id}` | Update an existing trade |

### Opportunities — `/api/opportunities`

| Method | Path | Description |
|---|---|---|
| `GET` | `/api/opportunities` | List opportunities. Filters: `limit`, `page`, `symbol`, `isApproved` |
| `POST` | `/api/opportunities` | Record a new opportunity |

### Portfolio Snapshots — `/api/portfolio-snapshots`

| Method | Path | Description |
|---|---|---|
| `GET` | `/api/portfolio-snapshots` | List snapshots. Filter: `limit` |
| `POST` | `/api/portfolio-snapshots` | Store a new portfolio snapshot |

### Trading Data — `/api/tradingdata`

High-level analytics and event-logging endpoints used by bots and dashboards.

**Reads**

| Method | Path | Description |
|---|---|---|
| `GET` | `/api/tradingdata/positions/open/count` | Number of currently open positions |
| `GET` | `/api/tradingdata/positions/open` | List of open positions |
| `GET` | `/api/tradingdata/trades/last` | Last N closed trades (`limit`, default 5) |
| `GET` | `/api/tradingdata/pnl/daily` | Realized P&L for today (`quoteAsset`) |
| `GET` | `/api/tradingdata/pnl/total` | All-time realized P&L (`quoteAsset`) |
| `GET` | `/api/tradingdata/pnl/summary` | P&L breakdown: today, yesterday, week, month, year, total |
| `GET` | `/api/tradingdata/stats` | Win rate, trade counts, and P&L by period |
| `GET` | `/api/tradingdata/opportunities/recent` | Opportunities in the last N hours (`hours`, default 1) |

**Writes (event logging)**

| Method | Path | Description |
|---|---|---|
| `POST` | `/api/tradingdata/trades/open` | Log a trade open event |
| `POST` | `/api/tradingdata/trades/close` | Log a trade close event with P&L |
| `POST` | `/api/tradingdata/trades/takeprofit` | Update take-profit on an open trade |
| `POST` | `/api/tradingdata/opportunities` | Log a detected trading opportunity |
| `POST` | `/api/tradingdata/portfolio/snapshot` | Store a portfolio state snapshot |

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) (for containerized runs and integration tests)
- PostgreSQL instance (or use Docker Compose)

### Run locally

1. **Clone the repository**

   ```bash
   git clone https://github.com/dot-trading/trading-project-persistence.git
   cd trading-project-persistence
   ```

2. **Configure the database connection**

   The service reads connection settings from environment variables:

   | Variable | Default | Description |
   |---|---|---|
   | `POSTGRES_HOST` | `localhost` | PostgreSQL host |
   | `POSTGRES_PORT` | `5432` | PostgreSQL port |
   | `POSTGRES_PASSWORD` | _(required)_ | Database password |

   For local development you can set these in `appsettings.Development.json` or export them in your shell.

3. **Start a local PostgreSQL instance**

   ```bash
   docker run -d \
     --name trading-db \
     -e POSTGRES_USER=trading \
     -e POSTGRES_PASSWORD=trading_secure_pwd_2026 \
     -e POSTGRES_DB=trading \
     -p 5432:5432 \
     postgres:16
   ```

4. **Run the API**

   ```bash
   dotnet run --project src/TradingProject.Persistence.Api
   ```

   Swagger UI is available at `http://localhost:5000/api/docs` in Development mode.

### Run with Docker

```bash
docker build -t trading-project-persistence .
docker run -p 8080:8080 \
  -e POSTGRES_HOST=<host> \
  -e POSTGRES_PORT=5432 \
  -e POSTGRES_PASSWORD=<password> \
  trading-project-persistence
```

---

## Testing

```bash
# Unit and application tests
dotnet test tests/TradingProject.Persistence.Application.Tests

# API layer tests
dotnet test tests/TradingProject.Persistence.Api.Tests

# Integration tests (requires Docker for Testcontainers)
dotnet test tests/TradingProject.Persistence.IntegrationTests
```

Integration tests use [Testcontainers](https://dotnet.testcontainers.org/) to spin up a real PostgreSQL instance automatically — no manual setup needed.

---

## Deployment

The service ships as a Docker image (`hsaii/trading-project-persistence:latest`) and is deployed to Kubernetes via manifests in the `/k8s` directory.

### Kubernetes configuration

- **Namespace**: `trading-ai`
- **Service type**: `ClusterIP` (internal only, port 80 → container 8080)
- **ConfigMap**: `service-endpoints` — service discovery for other platform services
- **Secret**: `trading-secrets` — PostgreSQL credentials
- **Health check**: `GET /api/TradingData/stats` (liveness probe, 20 s interval)
- **Resources**: 100m–250m CPU / 128Mi–256Mi memory

### CI/CD

GitHub Actions (`.github/workflows/ci.yml`) handles the full pipeline:

1. **Build & test** — every push and pull request
2. **Docker build & push** — on merge to `main`, tagged with commit SHA and `latest`
3. **Kubernetes rollout** — applies manifests and waits for rollout completion (120 s timeout)

---

## Domain Entities

### Trade
Represents a single trading position from open to close.

| Field | Type | Description |
|---|---|---|
| `Symbol` | string | Trading pair (e.g. `BTCUSDT`) |
| `Side` | string | `BUY` or `SELL` |
| `Status` | string | `open` or `closed` |
| `Price` | double | Entry price |
| `Quantity` | double | Position size |
| `Value` | double | Position value in quote asset |
| `StopLoss` | double? | Stop-loss price level |
| `TakeProfit` | double? | Take-profit price level |
| `AiScore` | int? | Confidence score from the AI agent (0–100) |
| `ClosePrice` | double? | Exit price (set on close) |
| `Pnl` | double? | Realized profit/loss in quote asset |
| `PnlPct` | double? | Realized profit/loss as percentage |

### Opportunity
Represents a trading signal detected by the AI agent before a decision is made.

| Field | Type | Description |
|---|---|---|
| `Symbol` | string | Trading pair |
| `Score` | int | Confidence score (0–100) |
| `Signal` | string | Direction of the signal |
| `Reason` | string | Human-readable explanation |
| `TargetPct` | double? | Expected gain percentage |
| `StopLossPct` | double? | Maximum acceptable loss percentage |
| `Price` | double | Price at signal time |
| `Acted` | bool | Whether the bot acted on this opportunity |
| `IsApproved` | bool | Whether the signal passed validation |
| `ValidationReason` | string? | Reason if rejected by validation |

### PortfolioSnapshot
A point-in-time capture of the portfolio state.

| Field | Type | Description |
|---|---|---|
| `Total` | double | Total portfolio value in quote asset |
| `Free` | double | Available (unallocated) balance |
| `PositionsCount` | int | Number of open positions at snapshot time |
| `DailyPnl` | double | Realized P&L for the day at snapshot time |
| `TotalPnl` | double | All-time realized P&L at snapshot time |

---

## Contributing

1. Fork the repository and create a feature branch.
2. Follow the existing Clean Architecture layer boundaries — domain entities have no external dependencies, application layer has no infrastructure dependencies.
3. Add tests for new use cases. Integration tests are preferred for database-touching logic.
4. Open a pull request against `main`. The CI pipeline will run automatically.

---

## Donate

If this project is useful to you, consider supporting its development.

**EVM (ETH / BNB / MATIC / any EVM-compatible chain)**
```
0x923ff4cdf36f6b3fe292390d26a3e145df8733f2
```

---

## License

Part of the [dot-trading](https://github.com/dot-trading) open-source trading platform.
