## Setup

### 1. Start the database

```bash
docker-compose up -d
```

### 2. Apply migrations

```bash
dotnet ef database update --project src/Librarium.Data --startup-project src/Librarium.Api
```

### 3. Start the API

```bash
dotnet run --project src/Librarium.Api
```

---

## Project Structure

```md
src/
  Librarium.Api/       — ASP.NET Core minimal API (endpoints, services, DTOs)
  Librarium.Data/      — EF Core data layer (entities, repositories, migrations)
migrations/
  sql/                 — Hand-maintained SQL migration artifacts (one file per migration)
  README.md            — Migration log with decisions and deployment notes
docs/                  — Task implementation notes
scripts/
  New-Migration.ps1    — Helper script for creating a new EF migration and its SQL artifact
```
