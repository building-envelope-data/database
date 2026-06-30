START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260626170958_AddSolarScreenToComponentSubtype') THEN
    ALTER TYPE database.optical_component_subtype ADD VALUE 'solar_screen' AFTER 'shade_material';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260626170958_AddSolarScreenToComponentSubtype') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260626170958_AddSolarScreenToComponentSubtype', '10.0.9');
    END IF;
END $EF$;

COMMIT;

