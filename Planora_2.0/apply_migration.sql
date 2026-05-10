START TRANSACTION;
ALTER TABLE "Schedules" ADD "IsOnline" boolean NOT NULL DEFAULT FALSE;
ALTER TABLE "Schedules" ADD "MeetingLink" text;
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260503201845_AddIsOnlineToSchedule', '10.0.5');
COMMIT;
