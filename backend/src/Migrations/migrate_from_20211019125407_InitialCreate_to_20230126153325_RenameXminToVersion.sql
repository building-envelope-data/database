START TRANSACTION;

ALTER TABLE database."photovoltaic_data_Sources" ALTER COLUMN "Value_DataTimestamp" TYPE timestamp with time zone;

ALTER TABLE database."photovoltaic_data_Approvals" ALTER COLUMN "Timestamp" TYPE timestamp with time zone;

ALTER TABLE database.photovoltaic_data ALTER COLUMN "CreatedAt" TYPE timestamp with time zone;

ALTER TABLE database."optical_data_Sources" ALTER COLUMN "Value_DataTimestamp" TYPE timestamp with time zone;

ALTER TABLE database."optical_data_Approvals" ALTER COLUMN "Timestamp" TYPE timestamp with time zone;

ALTER TABLE database.optical_data ALTER COLUMN "CreatedAt" TYPE timestamp with time zone;

ALTER TABLE database."hygrothermal_data_Sources" ALTER COLUMN "Value_DataTimestamp" TYPE timestamp with time zone;

ALTER TABLE database."hygrothermal_data_Approvals" ALTER COLUMN "Timestamp" TYPE timestamp with time zone;

ALTER TABLE database.hygrothermal_data ALTER COLUMN "CreatedAt" TYPE timestamp with time zone;

ALTER TABLE database."calorimetric_data_Sources" ALTER COLUMN "Value_DataTimestamp" TYPE timestamp with time zone;

ALTER TABLE database."calorimetric_data_Approvals" ALTER COLUMN "Timestamp" TYPE timestamp with time zone;

ALTER TABLE database.calorimetric_data ALTER COLUMN "CreatedAt" TYPE timestamp with time zone;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20230126153325_RenameXminToVersion', '7.0.2');

COMMIT;

