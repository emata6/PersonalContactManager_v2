# Personal Contact Manager

A full-stack contact management application built with **.NET 10** and **Angular 21**, following Clean Architecture and modern software engineering practices.

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
git clone https://github.com/emata6/PersonalContactManager_v2.git
cd PersonalContactManager_v2
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

A seed file with 24 fictional contacts is included at `PersonalContactManager.Api/seed-contacts.csv`.

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

### Redis with HybridCache
HybridCache (new in .NET 9) provides a two-tier cache: an in-process L1 cache for the fastest possible reads, backed by Redis as an L2 distributed cache so multiple API instances share the same cached data. Frequently read data (contacts list, tags, groups) is served from memory rather than hitting SQL Server on every request.

### Hangfire
Hangfire provides persistent background job scheduling backed by SQL Server. Jobs survive application restarts — if the API goes down and comes back up, scheduled birthday reminders are not lost. The built-in dashboard at `/hangfire` gives visibility into job history without any additional tooling.

### SignalR instead of polling
Polling means every client hammers the API on a timer regardless of whether anything changed. SignalR maintains a persistent WebSocket connection and pushes events only when something actually happens. The result is instant updates in the browser (reminder toasts, contact list refreshes) with zero unnecessary traffic.

### DirectEventDispatcher instead of a message broker
A message broker (like RabbitMQ) adds significant operational complexity — another service to run, configure, and monitor — that is only justified when services need to communicate across process boundaries. Since all event consumers (SignalR, email) live in the same process, dispatching domain events in-process using `IServiceScopeFactory` achieves the same decoupling with no infrastructure overhead.

### Docker Compose for local development
Docker Compose lets any developer clone the repository and run the entire stack — SQL Server, Redis, API, and frontend — with a single `docker compose up`. There are no local install prerequisites beyond Docker Desktop, no version conflicts, and no manual database setup. The API applies migrations automatically on startup.

---

## AI Assistance

AI assistance was used for implementation ideas and some project setup.

---

## Future Improvements

- **Authentication & authorisation** — JWT-based auth so each user manages their own contacts privately
- **Audit log** — record who changed what and when, using domain events already in place
- **Recurring reminders** — extend the reminder model to support daily/weekly/monthly recurrence rules
