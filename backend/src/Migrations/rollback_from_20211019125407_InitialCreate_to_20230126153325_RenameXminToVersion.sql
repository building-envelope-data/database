START TRANSACTION;

ALTER TABLE database."photovoltaic_data_Sources" ALTER COLUMN "Value_DataTimestamp" TYPE timestamp without time zone;

ALTER TABLE database."photovoltaic_data_Approvals" ALTER COLUMN "Timestamp" TYPE timestamp without time zone;

ALTER TABLE database.photovoltaic_data ALTER COLUMN "CreatedAt" TYPE timestamp without time zone;

ALTER TABLE database."optical_data_Sources" ALTER COLUMN "Value_DataTimestamp" TYPE timestamp without time zone;

ALTER TABLE database."optical_data_Approvals" ALTER COLUMN "Timestamp" TYPE timestamp without time zone;

ALTER TABLE database.optical_data ALTER COLUMN "CreatedAt" TYPE timestamp without time zone;

ALTER TABLE database."hygrothermal_data_Sources" ALTER COLUMN "Value_DataTimestamp" TYPE timestamp without time zone;

ALTER TABLE database."hygrothermal_data_Approvals" ALTER COLUMN "Timestamp" TYPE timestamp without time zone;

ALTER TABLE database.hygrothermal_data ALTER COLUMN "CreatedAt" TYPE timestamp without time zone;

ALTER TABLE database."calorimetric_data_Sources" ALTER COLUMN "Value_DataTimestamp" TYPE timestamp without time zone;

ALTER TABLE database."calorimetric_data_Approvals" ALTER COLUMN "Timestamp" TYPE timestamp without time zone;

ALTER TABLE database.calorimetric_data ALTER COLUMN "CreatedAt" TYPE timestamp without time zone;

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20230126153325_RenameXminToVersion';

COMMIT;

