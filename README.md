# 🎬 CinemaSystem

A full-stack cinema booking and management platform built with **Blazor Server**, **ASP.NET Core**, and **PostgreSQL**. Started as a database design course project, grown into a portfolio-ready web application with real-time seat booking, an admin dashboard, and SQL-driven analytics.

<!-- Replace with an actual banner/gif of the app in action -->
![Demo](/demo.gif)

---

## ✨ Features

### 🛠️ Admin Panel
- Full CRUD for movies, genres, halls, seats, sessions, customers, tickets and payments
- Automatic seat-grid generation when creating a hall, with row-based pricing
- Schedule conflict detection — no two sessions in the same hall at the same time
- Friendly error handling for DB-level integrity constraints (no raw SQL errors shown to the user)

### 🔎 Search & Filtering
- Movies by title (partial match) and genre
- Sessions by date
- Tickets by customer and status
- All filtering happens at the query level (`IQueryable` + LINQ), not in memory

### 📊 Analytics & Reports
- Revenue, tickets sold, average ticket price per movie
- Most popular movie, hall occupancy
- Daily revenue with running totals (SQL window functions)
- Raw SQL via **Dapper**, reusing hand-written analytical queries (CTEs, `RANK() OVER`, etc.)

### 🎟️ Customer-facing booking *(in progress)*
- Movie listing & showtimes
- Interactive seat map per session
- Real-time seat locking via **SignalR** — no double-booking, even with concurrent users
- Booking flow wrapped in a DB transaction, safely handling the `UNIQUE(session_id, seat_id)` constraint

---

## 🧱 Tech Stack

<div align="center">

| Layer | Technology |
|---|---|
| UI | Blazor Server, [MudBlazor](https://mudblazor.com/) |
| Backend | ASP.NET Core |
| Data access | Entity Framework Core (Npgsql) + Dapper for analytical queries |
| Database | PostgreSQL |
| Real-time | SignalR |
| Auth | Cookie-based authentication, role-based authorization |

</div>

## 🏗️ Architecture

Three-layer solution with one-way dependency flow: UI → Service → Repository → Database. Database-first approach — the schema was designed and populated directly in PostgreSQL, then scaffolded into C# via EF Core.

```
CinemaSystem.sln
│
├── Cinema.Domain
│   Entities, interfaces, DTOs
│
├── Cinema.Infrastructure
│   DbContext, repositories, Dapper reports
│
└── Cinema.Web
    Blazor Server UI, services, SignalR
```

---


## 🚀 Live Demo

Try it here: **[cinema-system.example.com](#)** *(link coming soon)*

**Admin login:**
- Login: `admin`
- Password: `ChangeMe123!`

> This is a public demo environment with sample data — feel free to add, edit, or delete records. Everything resets periodically.

---

## 🏃 Running locally

**Prerequisites:** .NET 9/10 SDK, PostgreSQL 16+

```bash
git clone https://github.com/ValeriaKarpachova/CinemaSystem.git
cd CinemaSystem

# restore a PostgreSQL database named "cinema" using db/schema.sql + db/seed.sql
psql -U postgres -f db/schema.sql
psql -U postgres -f db/seed.sql

# set your connection string
dotnet user-secrets set "ConnectionStrings:CinemaDb" "Host=localhost;Database=cinema;Username=postgres;Password=yourpassword" --project Cinema.Web

dotnet run --project Cinema.Web
```

Then open `https://localhost:xxxx/admin` and log in with the demo credentials above (or create your own admin via `/setup-admin` on first run).

---

## 📄 License

MIT — feel free to fork and build on it.
