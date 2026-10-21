# Mapa de funcionalidades: Manga Whisper

O mapa do projeto: como instalar e rodar, como acionar cada funcionalidade, onde está o código e qual teste a cobre. Um agente que for mexer no projeto, verificar uma entrega ou investigar um bug começa por aqui, em vez de adivinhar. Caminhos a partir da raiz do repositório. Escrito pela skill `onboard`; a etapa `implement` do pipeline o mantém em dia.

Este repositório é metade do sistema: o scraper é o `../manga-whisper-background-worker` (repositório separado, com o próprio `docs/FEATURE-MAP.md`). Os dois só se falam pelo banco.

## Instalar e rodar
- Requisitos: Docker Desktop; os dois repositórios como pastas irmãs; `.env` na raiz (cópia de `.env.example`). Fora do Docker: .NET SDK 10 (verificado com 10.0.102) e Node + Angular CLI 20
- Instalar: `cp .env.example .env`; front-end fora do Docker: `cd front-end && npm install`
- Rodar tudo: `docker compose up --build` → front em http://localhost:4200, API em http://localhost:5065, PostgreSQL em `localhost:5432`. **Não verificado: o Docker Desktop não estava rodando no onboard (2026-10-20)**
- Rodar a API fora do Docker: `docker compose up db`, copiar `back-end/MangaWhisper.Api/.env.example` para `.env`, `cd back-end/MangaWhisper.Api && dotnet run` (http://localhost:5065). Não verificado (precisa do banco)
- Rodar o front fora do Docker: `cd front-end && npm start` (http://localhost:4200, chama a API em http://localhost:5065). Não verificado
- Build da API: `dotnet build manga-whisper.sln` → "Compilação com êxito", 0 avisos, 0 erros (verificado)
- Build do front: `cd front-end && npx ng build` → bundle gerado em `front-end/dist/front-end`, 417 kB inicial (verificado)
- Testes: **não há**. Nenhum projeto de teste .NET e nenhum `*.spec.ts` (o `ng test` com Karma está configurado, mas sem specs)
- Banco: `docker compose exec db psql -U postgres -d manga_whisper`; recriar do zero: `docker compose down -v && docker compose up --build` (apaga os dados)

## Arquitetura em uma linha por camada
- `back-end/MangaWhisper.Api`: controllers (`Controllers/`), `Program.cs` (DI, JWT, CORS, OpenAPI, `DatabaseSeeder` no start).
- `back-end/MangaWhisper.Application`: CQRS com MediatR. Escrita = `Commands/*CommandHandler` (EF Core); leitura = `Queries/*QueryHandler` (Dapper); `Mappers/` manuais; `Scheduling/JobSchedule.cs` (cron em BRT).
- `back-end/MangaWhisper.Domain`: entidades e interfaces de repositório. `MangaWhisper.Infrastructure`: `ApplicationDbContext` (EF), `QueryDbContext` (Dapper), repositórios, `DatabaseSeeder`. `MangaWhisper.Common`: DTOs e enums.
- `database/init/01-schema.sql` e `02-seed.sql`: o esquema. **Sem migrations do EF**: entidade mudou → mudar o SQL aqui e as entidades nos **dois** repositórios.
- `front-end/src/app`: Angular 20 standalone com signals. Rotas em `app.routes.ts`; chamadas HTTP nos `*.service.ts`; `shared/core/auth.interceptor.ts` põe o JWT; `admin.guard.ts` protege o painel.
- Regras do projeto: `.github/instructions/backend.instructions.md` e `frontend.instructions.md`.

## Funcionalidades
| Funcionalidade | Como acionar (comando, rota, tela → clique, atalho) | Código | Teste |
|----------------|------------------------------------------------------|--------|-------|
| Ver capítulos recentes (One Piece) | http://localhost:4200/ → lista de capítulos recentes | front: `one-piece-tracker/one-piece-tracker.component.ts`, `components/recent-chapters/`, `services/chapter.service.ts`; API: `GET /api/chapters?page=&pageSize=&mangaTitle=` › `ChaptersController.GetChapters` › `GetChaptersQueryHandler` | — |
| Espalhamento dos lançamentos | http://localhost:4200/ → componente de release spread | `one-piece-tracker/components/release-spread/release-spread.component.ts` | — |
| Checker de um mangá (público) | `GET /api/mangacheckers/get-checker-by-manga-title/{mangaTitle}` | `MangaCheckersController.GetCheckerByMangaTitle` › `GetMangaCheckerByMangaTitleQueryHandler` | — |
| Login do admin (JWT + Identity) | http://localhost:4200/admin → e-mail e senha de `ADMIN_EMAIL`/`ADMIN_PASSWORD` do `.env`; `POST /api/auth/login` | front: `admin-login/admin-login.ts`, `admin-login.service.ts`, `shared/core/auth.service.ts`; API: `AuthController.Login` | — |
| Logout | botão no header do painel; `POST /api/auth/logout` | `shared/components/header/`; `AuthController.Logout` | — |
| Proteger o painel | abrir http://localhost:4200/admin-panel sem login → volta para o login | `shared/core/admin.guard.ts`; `[Authorize(Roles = "ADMIN")]` em `AdminController` e `AdminJobsController` | — |
| Listar checkers | painel → aba **Checkers** (`/admin-panel/checkers`); `GET /api/admin/checkers` | `admin-panel/pages/admin-checkers/`, `components/checker-card/`; `AdminController.GetCheckers` › `GetAdminCheckersQueryHandler` | — |
| Editar checker (último capítulo conhecido, pausar) | aba Checkers → card do checker → salvar; `PATCH /api/admin/checkers/{id}` | `checker-card.ts`; `AdminController.UpdateChecker` › `UpdateCheckerCommandHandler` | — |
| Run now (Check next chapter / Backfill) | aba Checkers → card → **Run now** → tipo; `POST /api/admin/checkers/{id}/runs` | `checker-card.ts`, `admin-panel-api.service.ts`; `AdminJobsController.RequestRun` › `RequestJobRunCommandHandler` (cria `JobRuns` Pending; o worker executa) | — |
| Jobs agendados (criar, editar, apagar) | aba **Scheduled jobs** (`/admin-panel/jobs`) → formulário; `GET/POST /api/admin/jobs`, `PUT/DELETE /api/admin/jobs/{id}` | `pages/admin-jobs/`, `components/job-form/`, `schedule.ts`, `job-labels.ts`; `AdminJobsController` › `SaveScrapingJobCommandHandler`, `DeleteScrapingJobCommandHandler`, `GetScrapingJobsQueryHandler`; `Application/Scheduling/JobSchedule.cs` (próxima execução em BRT) | — |
| Histórico de execuções | aba Scheduled jobs → histórico no fim da página; `GET /api/admin/runs?take=20` | `admin-jobs`; `AdminJobsController.GetRuns` › `GetJobRunsQueryHandler` | — |
| Status do worker | painel → indicador do worker; `GET /api/admin/worker-status` (lê `WorkerHeartbeats`) | `components/worker-status/worker-status.ts`; `AdminJobsController.GetWorkerStatus` › `GetWorkerStatusQueryHandler` | — |
| Criar papéis e o admin no start | automático ao subir a API | `Infrastructure/Data/DatabaseSeeder.cs` (chamado em `Program.cs`) | — |
| Documento OpenAPI | http://localhost:5065/openapi/v1.json (só em Development) | `Program.cs` › `MapOpenApi` | — |

## Entradas e saídas
- Variáveis (`.env` da raiz, passadas pelo `docker-compose.yml`): `POSTGRES_DB`, `POSTGRES_USER`, `POSTGRES_PASSWORD`, `JWT_SECRET` (32+ caracteres), `JWT_ISSUER`, `JWT_AUDIENCE`, `ADMIN_EMAIL`, `ADMIN_PASSWORD` (8+ caracteres, maiúscula, minúscula, dígito e símbolo). A API recebe `DefaultConnection`.
- Portas: 4200 (front, nginx com `/api` → container `api`), 5065 (API, 8080 no container), 5432 (PostgreSQL).
- Tabelas que importam para o worker: `MangaCheckers`, `ScrapingJobs` (cron), `JobRuns`, `Chapters`, `WorkerHeartbeats`. A API escreve jobs e pedidos de execução; o worker os executa e grava capítulos.
- O PostgreSQL só roda `database/init/*.sql` quando o volume `db-data` está vazio.

## Erros conhecidos e códigos de saída
- Mudança de esquema não aparece → o volume já existia: `docker compose down -v` (apaga os dados) ou `ALTER TABLE` à mão + atualizar `01-schema.sql`.
- Admin não consegue entrar → senha do `.env` fora das regras do Identity: o `DatabaseSeeder` não cria o usuário.
- `npm start` no front com o container `front-end` rodando → porta 4200 ocupada.
- Execução agendada que caiu enquanto o worker estava parado não é refeita (regra do worker): use **Run now**.
- Sem testes automatizados: a etapa `implement` do pipeline (TDD) precisa criar o primeiro projeto de teste (xUnit para o back, specs para o front) na primeira fatia.

## Skills do projeto
Além das skills do pipeline. Buscas feitas com a `find-skills` (2026-10-20): `xunit`, `dotnet test`, `selenium`, `docker compose`, `aspnet core security`, `jwt authentication`, `mediatr cqrs`, `dapper`.

| Skill | Fonte | Quando usar |
|-------|-------|-------------|
| `angular-developer` | `angular/skills` | código Angular do `front-end/` (oficial) |
| `frontend-design` | `anthropics/skills` | dar às telas um visual próprio |
| `web-design-guidelines` | `vercel-labs/agent-skills` | revisar UI/UX e acessibilidade do front |
| `dotnet-webapi` | `dotnet/skills` | endpoints da API (oficial) |
| `optimizing-ef-core-queries` | `dotnet/skills` | queries e repositórios EF Core (oficial) |
| `run-tests` | `dotnet/skills` | o comando `dotnet test` certo, filtros, cobertura (oficial) |
| `csharp-xunit` | `github/awesome-copilot` | escrever os testes xUnit (o projeto ainda não tem nenhum) |
| `postgresql-optimization` | `github/awesome-copilot` | esquema e SQL de `database/init/` |

Recusadas: `selenium-automation` (`mindrally/skills`), feita para Python/pytest; `dotnet-backend-patterns` (`wshobson/agents`), genérica e repete `dotnet-webapi`, EF Core e async; `test-anti-patterns` (`dotnet/skills`), só audita testes que já existem. Para JWT, MediatR e Docker não apareceu nenhuma com fonte e instalações confiáveis.
