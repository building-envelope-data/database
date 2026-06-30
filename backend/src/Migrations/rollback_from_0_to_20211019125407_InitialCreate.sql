START TRANSACTION;

DROP TABLE database."calorimetric_data_Approvals";

DROP TABLE database."calorimetric_data_Arguments";

DROP TABLE database."calorimetric_data_Sources";

DROP TABLE database."CielabColor";

DROP TABLE database."FileMetaInformation";

DROP TABLE database."get_https_resource_Arguments";

DROP TABLE database."hygrothermal_data_Approvals";

DROP TABLE database."hygrothermal_data_Arguments";

DROP TABLE database."hygrothermal_data_Sources";

DROP TABLE database."optical_data_Approvals";

DROP TABLE database."optical_data_Arguments";

DROP TABLE database."optical_data_Sources";

DROP TABLE database."photovoltaic_data_Approvals";

DROP TABLE database."photovoltaic_data_Arguments";

DROP TABLE database."photovoltaic_data_Sources";

DROP TABLE database.get_https_resource;

DROP TABLE database.calorimetric_data;

DROP TABLE database.hygrothermal_data;

DROP TABLE database.optical_data;

DROP TABLE database.photovoltaic_data;

DROP EXTENSION IF EXISTS pgcrypto;

DROP TYPE public.data_kind;

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20211019125407_InitialCreate';

COMMIT;

