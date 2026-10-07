-- Initial data. Runs once, right after 01-schema.sql, on an empty data volume.
-- Roles and the admin user are NOT seeded here: MangaWhisper.Api seeds them on startup
-- (DatabaseSeeder) because the password must be hashed by ASP.NET Identity.
--
-- Enum values (MangaWhisper.Common.Enums):
--   MangaStatus.Ongoing = 1
--   MangaCheckerStatus.Idle = 2

INSERT INTO "Mangas" ("Title", "CoverImageUrl", "Status", "CreatedAt", "UpdatedAt")
VALUES ('One Piece', '', 1, now(), now());

INSERT INTO "MangaCheckers" ("MangaId", "LastKnownChapter", "CheckIntervalMinutes", "IsActive", "CheckerStatus", "CreatedAt", "SiteIdentifier")
SELECT "Id", 0, 1, true, 2, now(), 'mugiwara-oficial'
FROM "Mangas"
WHERE "Title" = 'One Piece';
