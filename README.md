# Manga Whisper
A voz suave que te conta quando chega um capítulo

## 🧩 How the pieces fit together

| Service | Source | Role |
| --- | --- | --- |
| `front-end` | `front-end/` (Angular) | UI, served by nginx. `/api` is proxied to the API container |
| `api` | `back-end/` (ASP.NET Core) | Reads chapters, authentication (JWT + Identity) |
| `worker` | `../manga-whisper-background-worker` (separate repo) | Scrapes new chapters with Selenium and writes them to the database |
| `db` | `database/init/` (SQL) | PostgreSQL shared by `api` and `worker` |

The API and the worker never call each other: they only share the database.

## 🐳 Local setup (Docker)

### Prerequisites

- Docker Desktop
- Both repositories cloned as sibling folders:

```text
<parent>/
├── manga-whisper/                    (this repo)
└── manga-whisper-background-worker/
```

### Step 1: Create the `.env` file

In the root of this repo, make a copy of `.env.example` and rename it to `.env`. Adjust the values if you want (passwords, admin user, worker schedule).

### Step 2: Start everything

```bash
docker compose up --build
```

| URL | What |
| --- | --- |
| http://localhost:4200 | Front-end |
| http://localhost:4200/admin | Admin login (`ADMIN_EMAIL` / `ADMIN_PASSWORD` from `.env`) |
| http://localhost:5065/api/chapters | API (directly) |
| http://localhost:5065/openapi/v1.json | OpenAPI document |
| `localhost:5432` | PostgreSQL (for your DB client) |

## 🗄️ Database (no migrations)

The project does **not** use EF Core migrations. The schema is plain SQL:

- `database/init/01-schema.sql`: tables and indexes
- `database/init/02-seed.sql`: initial data (One Piece + its checker)

PostgreSQL runs these scripts **only when its data volume is empty** (first start). Roles and the admin user are created by the API on startup (`DatabaseSeeder`), because the password must be hashed by ASP.NET Identity.

When you change the schema, edit the SQL files, update the entities in **both** repositories, and re-create the database:

```bash
docker compose down -v
docker compose up --build
```

`-v` deletes the database volume (all data). To keep data, apply the change manually with your DB client instead (`ALTER TABLE ...`) and also add it to `01-schema.sql`.

## 🛠️ Useful commands

```bash
docker compose logs -f worker          # follow the scraper
docker compose logs -f api
docker compose up --build api          # rebuild/restart a single service
docker compose exec db psql -U postgres -d manga_whisper
docker compose down                    # stop (keeps data)
```

## 💻 Running a service outside Docker (optional)

Useful for debugging with breakpoints or `ng serve` hot reload. Keep the `db` container running (`docker compose up db`) and:

- **API**: copy `back-end/MangaWhisper.Api/.env.example` to `.env`, then `dotnet run` inside `back-end/MangaWhisper.Api` (listens on http://localhost:5065).
- **Front-end**: `npm install && npm start` inside `front-end` (http://localhost:4200, calls the API at http://localhost:5065). Stop the `front-end` container first, since it uses the same port.
