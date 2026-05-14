# Personal Contact Manager

A full-stack contact management application built with .NET 10 and Angular 21.

## Tech Stack

**Backend**
- .NET 10 Web API — Clean Architecture (Domain / Application / Infrastructure / API)
- CQRS with MediatR — commands and queries fully separated
- Rich Domain Model — business logic lives in entities, not handlers
- FluentValidation — pipeline behaviour validates every command before it reaches the handler
- EF Core 9 — SQL Server via Docker, code-first migrations
- SignalR — real-time reminder notifications
- Hangfire — background job scheduling for reminders
- RabbitMQ + MassTransit — message bus for email delivery

**Frontend**
- Angular 21 — standalone components, signals, `input.required<T>()`
- NgRx — feature stores for contacts, tags, groups, reminders, and dashboard
- PrimeNG 21 — component library (no custom CSS; layout uses inline styles with PrimeNG design tokens)
- HTTP error interceptor — centralised toast notifications for API errors
- Playwright — end-to-end tests

## Features

- **Contacts** — create, edit, delete with all required fields (first name, surname, DOB, address, phone number, IBAN)
- **Phone numbers** — multiple numbers per contact with labels (Mobile / Home / Work / Other)
- **Tags** — colour-coded labels assignable to contacts
- **Groups** — group contacts and browse members
- **Reminders** — schedule reminders per contact with in-app (SignalR), email, or both channels
- **Dashboard** — stat cards, recent contacts, upcoming birthdays
- **CSV import / export** — bulk import contacts from CSV; export the current filtered list
- **Favourites** — star contacts and filter by favourite
- **Search & filter** — search by name/email, filter by tag or group, sort by name or date

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

## Getting Started

### 1. Start infrastructure

```bash
docker compose up -d
```

This starts SQL Server on `localhost:1433`, Redis on `localhost:6379`, and RabbitMQ on `localhost:5672` (management UI at `localhost:15672`).

### 2. Run the API

```bash
cd PersonalContactManager.Api
dotnet run --project PersonalContactManager.Api
```

API runs at `http://localhost:5032`. Swagger UI is available at `http://localhost:5032/swagger`.

EF Core migrations are applied automatically on startup.

### 3. Run the frontend

```bash
cd PersonalContactManager.Web
npm install
npm start
```

App runs at `http://localhost:4200`.

## Project Structure

```
PersonalContactManager.Api/
├── PersonalContactManager.Domain/          # Entities, value objects, domain events
├── PersonalContactManager.Application/    # Commands, queries, DTOs, validators, behaviours
├── PersonalContactManager.Infrastructure/ # EF Core, repositories, migrations, email
└── PersonalContactManager.Api/            # Controllers, SignalR hub, middleware

PersonalContactManager.Web/
├── src/app/
│   ├── core/           # Models, services, HTTP interceptor
│   ├── features/       # Contacts, dashboard, groups, reminders, tags
│   ├── layout/         # Shell, sidebar
│   ├── shared/         # PageHeaderComponent
│   └── store/          # NgRx feature stores
└── e2e/                # Playwright tests
```

## Running Tests

```bash
# End-to-end (requires the app to be running on localhost:4200)
cd PersonalContactManager.Web
npm run e2e

# View test report
npm run e2e:report
```

## Connection Strings

Default development values (match the Docker Compose configuration):

| Service    | Value                                                                                          |
|------------|-----------------------------------------------------------------------------------------------|
| SQL Server | `Server=localhost,1433;Database=PersonalContacts;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;` |
| Redis      | `localhost:6379`                                                                              |
| RabbitMQ   | `localhost:5672` — user `guest` / pass `guest`                                               |
