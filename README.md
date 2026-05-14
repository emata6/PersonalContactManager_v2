# Personal Contact Manager

A full-stack contact management application built with **.NET 10** and **Angular 21**, following Clean Architecture and modern software engineering practices. The project serves as a portfolio piece demonstrating production-level patterns across the entire stack — from domain modelling and CQRS on the backend, to reactive state management and real-time updates on the frontend.

---

## Table of Contents

- [Features](#features)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [API Reference](#api-reference)
- [Design Decisions](#design-decisions)

---

## Features

### Contact Management
Create, edit, and delete contacts with full validation enforced at every layer. Each contact stores:
- First name, last name, email
- Date of birth
- IBAN (validated format, copyable to clipboard)
- Multiple phone numbers with labels (Mobile, Home, Work, Other)
- Full address (street, city, state, postal code, country)
- Personal notes

### Tags
Color-coded labels that can be assigned to contacts for visual categorisation. Tags are managed independently and reused across contacts.

### Groups
Contacts can be organised into named groups with an optional description. The groups view shows member counts and provides a members dialog to browse and navigate directly to any contact.

### Favorites
Any contact can be starred as a favorite. Favorites are surfaced separately on the dashboard for quick access.

### Reminders
Schedule reminders for any contact with a title, optional note, due date, and delivery channel:
- **In-app** — a real-time toast notification pushed via SignalR the moment the reminder fires
- **Email** — an email sent to the contact's address when the reminder is due
- **Both** — delivers over both channels simultaneously

Reminder statuses progress through `Pending → Fired → Dismissed / Cancelled`.

### Birthday Reminders
A Hangfire background job runs daily and automatically fires reminders for contacts whose birthday falls today, so no birthday is missed without any manual scheduling.

### CSV Import & Export
- **Import** — upload a 13-column CSV file to bulk-create contacts including phone numbers and addresses in one request
- **Export** — download all contacts as a CSV for backup or migration

### Dashboard
A summary view showing total contacts, upcoming birthdays, favorite contacts, and quick-access cards.

### Real-Time Notifications
SignalR pushes live events to every connected browser tab — contact created/updated/deleted events refresh the list automatically, and reminder fired events appear as warning toasts without requiring a page reload.

### Search & Pagination
The contacts list supports server-side search by name or email and paginated results to keep response times consistent as the dataset grows.

### Soft Delete
Contacts are never permanently deleted on the first delete action — they are soft-deleted (flagged with `IsDeleted`) and can be restored. A global EF Core query filter transparently excludes soft-deleted records from all queries.

---

## Tech Stack

### Backend

| Technology | Version | Role |
|---|---|---|
| .NET / ASP.NET Core | 10 | Web API framework |
| Entity Framework Core | 10 | ORM and database migrations |
| SQL Server | 2022 | Primary relational database |
| Redis | 7 | Distributed L2 cache (HybridCache) |
| MediatR | 12 | CQRS mediator |
| FluentValidation | 11 | Request validation pipeline |
| Hangfire | 1.8 | Background job scheduling |
| SignalR | built-in | Real-time WebSocket push |
| Scalar | latest | OpenAPI documentation UI |

### Frontend

| Technology | Version | Role |
|---|---|---|
| Angular | 21 | SPA framework |
| NgRx | 19 | Reactive state management |
| PrimeNG | 19 | UI component library |
| @microsoft/signalr | latest | Real-time client |
| TypeScript | 5.8 | Type-safe JavaScript |

### Infrastructure

| Technology | Role |
|---|---|
| Docker & Docker Compose | Containerised local stack (SQL Server, Redis, API, Web) |
| nginx | Serves the Angular SPA and reverse-proxies `/api` and `/hubs` to the API container |

---

## Architecture

### Backend — Clean Architecture

The backend is split into four projects with strict dependency rules:

```
PersonalContactManager.Domain          ← no external dependencies
PersonalContactManager.Application     ← depends on Domain only
PersonalContactManager.Infrastructure  ← depends on Application + Domain
PersonalContactManager.Api             ← depends on all three (composition root)
```

**Domain** contains entities, value objects, domain events, repository interfaces, and aggregate roots. No framework dependencies — pure C#.

**Application** contains all use cases as CQRS commands and queries handled by MediatR. Three pipeline behaviours run on every request: `ValidationBehavior` (FluentValidation), `LoggingBehavior`, and `TransactionBehavior` (unit of work + domain event dispatch).

**Infrastructure** implements the repository interfaces, EF Core `AppDbContext`, Hangfire jobs, the `DirectEventDispatcher` (dispatches domain events in-process to SignalR and email), and Redis caching via `HybridCache`.

**API** is the composition root: controllers, SignalR hub, CORS, and `Program.cs` wiring.

### Frontend — Feature-based Angular

```
src/app/
├── core/           ← models, services, HTTP clients, utilities
├── features/       ← contacts, dashboard, groups, reminders, tags
├── shared/         ← reusable components (PageHeader, FormLabel, ReminderDialog)
└── store/          ← NgRx slices per domain entity
```

Each feature has its own NgRx slice (actions, reducer, effects, selectors). Effects handle all HTTP calls and dispatch success/failure actions. Components are thin — they select from the store and dispatch actions, containing no business logic.

---

## Getting Started

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- That's it — everything else runs inside containers.

### Run

```bash
git clone https://github.com/EmaKyuchukova/personal-contact-manager.git
cd personal-contact-manager
docker compose up
```

| Service | URL |
|---|---|
| UI | http://localhost:4200 |
| API | http://localhost:5032 |
| API Docs (Scalar) | http://localhost:5032/scalar/v1 |
| Hangfire Dashboard | http://localhost:5032/hangfire |

The API automatically applies EF Core migrations on startup — no manual database setup needed.

### Load Sample Data

A seed file with 24 fictional contacts (Avengers, DC, Sherlock Holmes, Supernatural) is included at `PersonalContactManager.Api/seed-contacts.csv`.

```bash
python3 -c "
import json
csv = open('PersonalContactManager.Api/seed-contacts.csv').read()
print(json.dumps({'csvContent': csv}))
" | curl -X POST http://localhost:5032/api/contacts/import \
  -H 'Content-Type: application/json' \
  -d @-
```

### Local Development (without Docker)

**API**
```bash
cd PersonalContactManager.Api
docker compose up sqlserver redis -d   # start only the infrastructure
dotnet run --project PersonalContactManager.Api
```

**Frontend**
```bash
cd PersonalContactManager.Web
npm install
ng serve
```

---

## Project Structure

```
PersonalContactManager/
├── docker-compose.yml
├── PersonalContactManager.Api/
│   ├── PersonalContactManager.Api/          ← controllers, middleware, SignalR hub
│   ├── PersonalContactManager.Application/  ← commands, queries, validators, DTOs
│   ├── PersonalContactManager.Domain/       ← entities, value objects, domain events
│   ├── PersonalContactManager.Infrastructure/  ← EF Core, repositories, jobs, cache
│   └── PersonalContactManager.Tests/
└── PersonalContactManager.Web/
    └── src/app/
        ├── core/
        ├── features/
        ├── shared/
        └── store/
```

---

## API Reference

Full interactive documentation is available at `/scalar/v1` when running locally.

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/contacts` | Paginated contacts list with search |
| POST | `/api/contacts` | Create a contact |
| GET | `/api/contacts/{id}` | Get contact details |
| PUT | `/api/contacts/{id}` | Update a contact |
| DELETE | `/api/contacts/{id}` | Soft-delete a contact |
| POST | `/api/contacts/{id}/toggle-favorite` | Star / unstar |
| POST | `/api/contacts/{id}/tags/{tagId}` | Assign tag |
| DELETE | `/api/contacts/{id}/tags/{tagId}` | Remove tag |
| POST | `/api/contacts/{id}/groups/{groupId}` | Assign to group |
| DELETE | `/api/contacts/{id}/groups/{groupId}` | Remove from group |
| POST | `/api/contacts/import` | Bulk import from CSV |
| GET | `/api/contacts/export` | Export all as CSV |
| GET | `/api/contacts/favorites` | List favorites |
| GET | `/api/contacts/upcoming-birthdays` | Upcoming birthdays |
| GET/POST | `/api/tags` | List / create tags |
| PUT/DELETE | `/api/tags/{id}` | Update / delete tag |
| GET/POST | `/api/groups` | List / create groups |
| PUT/DELETE | `/api/groups/{id}` | Update / delete group |
| GET | `/api/groups/{id}/contacts` | Members of a group |
| GET/POST | `/api/reminders` | List all / create reminder |
| GET | `/api/reminders/contact/{contactId}` | Reminders for a contact |
| POST | `/api/reminders/{id}/dismiss` | Dismiss a reminder |
| POST | `/api/reminders/{id}/cancel` | Cancel a reminder |

---

## Design Decisions

### Why Clean Architecture?
Clean Architecture enforces a dependency rule where inner layers know nothing about outer layers. This keeps the domain and business logic completely framework-agnostic and independently testable. Swapping the database provider or adding a new delivery mechanism (e.g. gRPC) requires no changes to the domain or application layers.

### Why CQRS with MediatR?
Separating commands (writes) from queries (reads) gives each operation a single, focused class. MediatR's pipeline makes it trivial to compose cross-cutting concerns — validation, logging, and transaction management run automatically on every request without polluting the handler. The result is handlers that are short, readable, and easy to test in isolation.

### Why FluentValidation?
FluentValidation keeps validation rules in dedicated classes rather than buried in controllers or domain methods. Rules are chainable, readable, and fully testable. The `ValidationBehavior` in the MediatR pipeline means validation always runs before a handler executes — no handler ever receives invalid input.

### Why Entity Framework Core?
EF Core handles the object-relational mapping, migrations, and owned entity types (Address is stored as columns on the Contacts table rather than a separate table). The global query filter for soft delete means deleted records are automatically excluded from every query without any developer effort.

### Why Redis with HybridCache?
HybridCache (new in .NET 9) provides a two-tier cache: an in-process L1 cache for the fastest possible reads, backed by Redis as an L2 distributed cache so multiple API instances share the same cached data. Frequently read data (contacts list, tags, groups) is served from memory rather than hitting SQL Server on every request.

### Why Hangfire?
Hangfire provides persistent background job scheduling backed by SQL Server. Jobs survive application restarts — if the API goes down and comes back up, scheduled birthday reminders are not lost. The built-in dashboard at `/hangfire` gives visibility into job history without any additional tooling.

### Why SignalR instead of polling?
Polling means every client hammers the API on a timer regardless of whether anything changed. SignalR maintains a persistent WebSocket connection and pushes events only when something actually happens. The result is instant updates in the browser (reminder toasts, contact list refreshes) with zero unnecessary traffic.

### Why DirectEventDispatcher instead of a message broker?
A message broker (like RabbitMQ) adds significant operational complexity — another service to run, configure, and monitor — that is only justified when services need to communicate across process boundaries. Since all event consumers (SignalR, email) live in the same process, dispatching domain events in-process using `IServiceScopeFactory` achieves the same decoupling with no infrastructure overhead.

### Why NgRx?
NgRx gives the Angular frontend a single predictable state tree. All server data lives in the store; components never fetch data directly. Effects handle side effects (HTTP calls, SignalR messages) in isolation from components. This architecture scales cleanly as the application grows and makes the data flow easy to follow and debug using NgRx DevTools.

### Why PrimeNG?
PrimeNG provides a comprehensive set of production-quality Angular components (data tables, dialogs, date pickers, color pickers, chips, tags) that are themeable and accessible out of the box. Building these from scratch would add weeks of work without adding architectural value to a portfolio project.

### Why Docker Compose for local development?
Docker Compose lets any developer clone the repository and run the entire stack — SQL Server, Redis, API, and frontend — with a single `docker compose up`. There are no local install prerequisites beyond Docker Desktop, no version conflicts, and no manual database setup. The API applies migrations automatically on startup.

---

## AI Assistance

This project was developed with the help of [Claude Code](https://claude.ai/code) (Anthropic). AI assistance was used for:

- **UI implementation ideas** — component structure, PrimeNG usage patterns, Angular signal-based patterns, and NgRx effect composition
- **Project setup** — Docker multi-stage build configuration, nginx reverse proxy setup, and docker-compose orchestration

All architectural decisions, domain modelling, and technology choices were made and understood by the developer. AI served as a productivity tool and sounding board, not as a replacement for engineering judgement.

---

## Future Improvements

Features and concerns that are out of scope for this portfolio project but would be addressed in a production system:

- **Rate limiting** — per-IP fixed-window limits on public endpoints to prevent abuse and brute-force attacks
- **Distributed tracing** — OpenTelemetry integration to trace requests across the API, database, and cache layers
- **Metrics & monitoring** — Prometheus/Grafana or Azure Monitor for request latency, error rates, and infrastructure health
- **Authentication & authorisation** — JWT-based auth so each user manages their own contacts privately
- **Audit log** — record who changed what and when, using domain events already in place
- **Recurring reminders** — extend the reminder model to support daily/weekly/monthly recurrence rules
