# Manga Whisper
A voz suave que te conta quando chega um capítulo

## 🧩 How the pieces fit together

| Service | Source | Role |
| --- | --- | --- |
| `front-end` | `front-end/` (Angular) | UI, served by nginx. `/api` is proxied to the API container |
| `api` | `back-end/` (ASP.NET Core) | Reads chapters, authentication (JWT + Identity), admin panel endpoints (checkers, scraping jobs) |
| `worker` | `../manga-whisper-background-worker` (separate repo) | Executes the scraping jobs created in the admin panel (Selenium) and writes the chapters to the database |
| `db` | `database/init/` (SQL) | PostgreSQL shared by `api` and `worker` |

The API and the worker never call each other: they only share the database. The API decides what to scrape (it writes jobs and "Run now" requests to the database) and the worker executes it.

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

In the root of this repo, make a copy of `.env.example` and rename it to `.env`. Adjust the values if you want (passwords, admin user). There are no schedule settings: scraping is configured in the admin panel.

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

## ⏰ Scraping jobs (admin panel)

Nothing is scraped when the containers start. Everything is configured in the **Control panel** (http://localhost:4200/admin-panel, admin only):

1. **Checkers** tab: set the **last known chapter** of the manga. Every check looks for the chapter after it.
2. **Checkers** tab: **Run now** queues a one-off run: *Check next chapter* (looks once for the next chapter) or *Backfill* (saves every chapter until one is missing).
3. **Scheduled jobs** tab: create a recurring job, e.g. *Check next chapter* every Thursday at 12, 16 and 20 (BRT). The run history at the bottom shows every run, its result and how many chapters it saved.

How it works:

- Jobs are stored as cron expressions (`ScrapingJobs` table) and evaluated in BRT. The API computes the next run when a job is saved; the worker polls the database every 30 seconds, turns due jobs into runs (`JobRuns` table) and executes them one at a time.
- The worker never catches up: if it is down at a scheduled time, that run is skipped, and runs requested while it was down are cancelled when it starts. Use **Run now** instead.
- A paused checker is skipped by its scheduled jobs; **Run now** still works for it.
- The panel shows whether the worker is running (it writes a heartbeat to `WorkerHeartbeats` every 30 seconds).

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

## 🤖 Agent skills

The repo uses the [skills pipeline](pipeline/README.md): an idea goes through steps (idea → decisions → PRD → architecture → TDD → verification → review → delivery), each one an [Agent Skill](https://www.skills.sh/). To build a feature, open the agent and say "run the pipeline". The map of every feature, its code and its test is [`docs/FEATURE-MAP.md`](docs/FEATURE-MAP.md).

The skills are listed in `pipeline/skills.json` (external, from skills.sh) and `pipeline/skills/` (the pipeline's own). The installed folders (`.claude/skills/`, `.agents/skills/`) are gitignored. After a clone, install them with (Node 22.18+):

```bash
npm --prefix pipeline install
npm --prefix pipeline run setup
```

Skills for this project, besides the pipeline's:

| Skill | Source | Use it for |
| --- | --- | --- |
| `frontend-design` | `anthropics/skills` | Giving the screens a distinctive visual design |
| `web-design-guidelines` | `vercel-labs/agent-skills` | Reviewing UI/UX and accessibility |
| `angular-developer` | `angular/skills` | Angular code and best practices (official) |
| `dotnet-webapi` | `dotnet/skills` | ASP.NET Core Web API endpoints (official) |
| `optimizing-ef-core-queries` | `dotnet/skills` | Reviewing EF Core queries and repositories (official) |
| `run-tests` | `dotnet/skills` | The right `dotnet test` command and filters (official) |
| `csharp-xunit` | `github/awesome-copilot` | Writing xUnit tests (the repo has none yet) |
| `postgresql-optimization` | `github/awesome-copilot` | PostgreSQL schema and SQL |
| `find-skills` | `vercel-labs/skills` | Finding more skills on skills.sh |

To add another one, put it in `pipeline/skills.json` and run `npm --prefix pipeline run setup`.
