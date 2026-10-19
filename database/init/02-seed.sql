-- Initial data. Runs once, right after 01-schema.sql, on an empty data volume.
-- Roles and the admin user are NOT seeded here: MangaWhisper.Api seeds them on startup
-- (DatabaseSeeder) because the password must be hashed by ASP.NET Identity.
--
-- Enum values (MangaWhisper.Common.Enums):
--   MangaStatus.Ongoing = 1
--   MangaCheckerStatus.Stopped = 1
--
-- The checker starts paused, with no scraping jobs: nothing is scraped until the admin sets the
-- last known chapter and creates a job (or clicks "Run now") in the admin panel.

INSERT INTO "Mangas" ("Title", "CoverImageUrl", "Status", "CreatedAt", "UpdatedAt")
VALUES ('One Piece', '', 1, now(), now());

INSERT INTO "MangaCheckers" ("MangaId", "LastKnownChapter", "IsActive", "CheckerStatus", "CreatedAt", "SiteIdentifier")
SELECT "Id", 0, false, 1, now(), 'mugiwara-oficial'
FROM "Mangas"
WHERE "Title" = 'One Piece';
