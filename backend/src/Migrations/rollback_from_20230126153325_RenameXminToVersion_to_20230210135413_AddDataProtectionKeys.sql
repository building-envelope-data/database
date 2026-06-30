START TRANSACTION;

DROP TABLE database."DataProtectionKeys";

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20230210135413_AddDataProtectionKeys';

COMMIT;

