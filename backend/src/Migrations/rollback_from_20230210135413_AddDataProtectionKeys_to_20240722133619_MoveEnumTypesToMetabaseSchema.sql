START TRANSACTION;

CREATE TYPE public.data_kind AS ENUM ('calorimetric_data', 'hygrothermal_data', 'optical_data', 'photovoltaic_data');
CREATE TYPE public.standardizer AS ENUM ('aerc', 'agi', 'ashrae', 'breeam', 'bs', 'bsi', 'cen', 'cie', 'dgnb', 'din', 'dvwg', 'iec', 'ies', 'ift', 'iso', 'jis', 'leed', 'nfrc', 'riba', 'ul', 'unece', 'vdi', 'vff', 'well');

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'database') THEN
        CREATE SCHEMA database;
    END IF;
END $EF$;

CREATE EXTENSION IF NOT EXISTS pgcrypto;

ALTER TABLE database."photovoltaic_data_Sources" ALTER COLUMN "Value_DataKind" TYPE data_kind USING "Value_DataKind"::text::data_kind;

ALTER TABLE database."photovoltaic_data_Approvals" ALTER COLUMN "Standard_Standardizers" TYPE standardizer[] USING "Standard_Standardizers"::text::standardizer[];

ALTER TABLE database."optical_data_Sources" ALTER COLUMN "Value_DataKind" TYPE data_kind USING "Value_DataKind"::text::data_kind;

ALTER TABLE database."optical_data_Approvals" ALTER COLUMN "Standard_Standardizers" TYPE standardizer[] USING "Standard_Standardizers"::text::standardizer[];

ALTER TABLE database."hygrothermal_data_Sources" ALTER COLUMN "Value_DataKind" TYPE data_kind USING "Value_DataKind"::text::data_kind;

ALTER TABLE database."hygrothermal_data_Approvals" ALTER COLUMN "Standard_Standardizers" TYPE standardizer[] USING "Standard_Standardizers"::text::standardizer[];

ALTER TABLE database."calorimetric_data_Sources" ALTER COLUMN "Value_DataKind" TYPE data_kind USING "Value_DataKind"::text::data_kind;

ALTER TABLE database."calorimetric_data_Approvals" ALTER COLUMN "Standard_Standardizers" TYPE standardizer[] USING "Standard_Standardizers"::text::standardizer[];

DROP TYPE database.data_kind;
DROP TYPE database.standardizer;

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20240722133619_MoveEnumTypesToMetabaseSchema';

COMMIT;

START TRANSACTION;

ALTER TABLE database."photovoltaic_data_Approvals" DROP COLUMN "Publication_Abstract";

ALTER TABLE database."photovoltaic_data_Approvals" DROP COLUMN "Publication_ArXiv";

ALTER TABLE database."photovoltaic_data_Approvals" DROP COLUMN "Publication_Authors";

ALTER TABLE database."photovoltaic_data_Approvals" DROP COLUMN "Publication_Doi";

ALTER TABLE database."photovoltaic_data_Approvals" DROP COLUMN "Publication_Section";

ALTER TABLE database."photovoltaic_data_Approvals" DROP COLUMN "Publication_Title";

ALTER TABLE database."photovoltaic_data_Approvals" DROP COLUMN "Publication_Urn";

ALTER TABLE database."photovoltaic_data_Approvals" DROP COLUMN "Publication_WebAddress";

ALTER TABLE database."photovoltaic_data_Approvals" DROP COLUMN "Standard_Abstract";

ALTER TABLE database."photovoltaic_data_Approvals" DROP COLUMN "Standard_Locator";

ALTER TABLE database."photovoltaic_data_Approvals" DROP COLUMN "Standard_Numeration_MainNumber";

ALTER TABLE database."photovoltaic_data_Approvals" DROP COLUMN "Standard_Numeration_Prefix";

ALTER TABLE database."photovoltaic_data_Approvals" DROP COLUMN "Standard_Numeration_Suffix";

ALTER TABLE database."photovoltaic_data_Approvals" DROP COLUMN "Standard_Section";

ALTER TABLE database."photovoltaic_data_Approvals" DROP COLUMN "Standard_Standardizers";

ALTER TABLE database."photovoltaic_data_Approvals" DROP COLUMN "Standard_Title";

ALTER TABLE database."photovoltaic_data_Approvals" DROP COLUMN "Standard_Year";

ALTER TABLE database."optical_data_Approvals" DROP COLUMN "Publication_Abstract";

ALTER TABLE database."optical_data_Approvals" DROP COLUMN "Publication_ArXiv";

ALTER TABLE database."optical_data_Approvals" DROP COLUMN "Publication_Authors";

ALTER TABLE database."optical_data_Approvals" DROP COLUMN "Publication_Doi";

ALTER TABLE database."optical_data_Approvals" DROP COLUMN "Publication_Section";

ALTER TABLE database."optical_data_Approvals" DROP COLUMN "Publication_Title";

ALTER TABLE database."optical_data_Approvals" DROP COLUMN "Publication_Urn";

ALTER TABLE database."optical_data_Approvals" DROP COLUMN "Publication_WebAddress";

ALTER TABLE database."optical_data_Approvals" DROP COLUMN "Standard_Abstract";

ALTER TABLE database."optical_data_Approvals" DROP COLUMN "Standard_Locator";

ALTER TABLE database."optical_data_Approvals" DROP COLUMN "Standard_Numeration_MainNumber";

ALTER TABLE database."optical_data_Approvals" DROP COLUMN "Standard_Numeration_Prefix";

ALTER TABLE database."optical_data_Approvals" DROP COLUMN "Standard_Numeration_Suffix";

ALTER TABLE database."optical_data_Approvals" DROP COLUMN "Standard_Section";

ALTER TABLE database."optical_data_Approvals" DROP COLUMN "Standard_Standardizers";

ALTER TABLE database."optical_data_Approvals" DROP COLUMN "Standard_Title";

ALTER TABLE database."optical_data_Approvals" DROP COLUMN "Standard_Year";

ALTER TABLE database."hygrothermal_data_Approvals" DROP COLUMN "Publication_Abstract";

ALTER TABLE database."hygrothermal_data_Approvals" DROP COLUMN "Publication_ArXiv";

ALTER TABLE database."hygrothermal_data_Approvals" DROP COLUMN "Publication_Authors";

ALTER TABLE database."hygrothermal_data_Approvals" DROP COLUMN "Publication_Doi";

ALTER TABLE database."hygrothermal_data_Approvals" DROP COLUMN "Publication_Section";

ALTER TABLE database."hygrothermal_data_Approvals" DROP COLUMN "Publication_Title";

ALTER TABLE database."hygrothermal_data_Approvals" DROP COLUMN "Publication_Urn";

ALTER TABLE database."hygrothermal_data_Approvals" DROP COLUMN "Publication_WebAddress";

ALTER TABLE database."hygrothermal_data_Approvals" DROP COLUMN "Standard_Abstract";

ALTER TABLE database."hygrothermal_data_Approvals" DROP COLUMN "Standard_Locator";

ALTER TABLE database."hygrothermal_data_Approvals" DROP COLUMN "Standard_Numeration_MainNumber";

ALTER TABLE database."hygrothermal_data_Approvals" DROP COLUMN "Standard_Numeration_Prefix";

ALTER TABLE database."hygrothermal_data_Approvals" DROP COLUMN "Standard_Numeration_Suffix";

ALTER TABLE database."hygrothermal_data_Approvals" DROP COLUMN "Standard_Section";

ALTER TABLE database."hygrothermal_data_Approvals" DROP COLUMN "Standard_Standardizers";

ALTER TABLE database."hygrothermal_data_Approvals" DROP COLUMN "Standard_Title";

ALTER TABLE database."hygrothermal_data_Approvals" DROP COLUMN "Standard_Year";

ALTER TABLE database."calorimetric_data_Approvals" DROP COLUMN "Publication_Abstract";

ALTER TABLE database."calorimetric_data_Approvals" DROP COLUMN "Publication_ArXiv";

ALTER TABLE database."calorimetric_data_Approvals" DROP COLUMN "Publication_Authors";

ALTER TABLE database."calorimetric_data_Approvals" DROP COLUMN "Publication_Doi";

ALTER TABLE database."calorimetric_data_Approvals" DROP COLUMN "Publication_Section";

ALTER TABLE database."calorimetric_data_Approvals" DROP COLUMN "Publication_Title";

ALTER TABLE database."calorimetric_data_Approvals" DROP COLUMN "Publication_Urn";

ALTER TABLE database."calorimetric_data_Approvals" DROP COLUMN "Publication_WebAddress";

ALTER TABLE database."calorimetric_data_Approvals" DROP COLUMN "Standard_Abstract";

ALTER TABLE database."calorimetric_data_Approvals" DROP COLUMN "Standard_Locator";

ALTER TABLE database."calorimetric_data_Approvals" DROP COLUMN "Standard_Numeration_MainNumber";

ALTER TABLE database."calorimetric_data_Approvals" DROP COLUMN "Standard_Numeration_Prefix";

ALTER TABLE database."calorimetric_data_Approvals" DROP COLUMN "Standard_Numeration_Suffix";

ALTER TABLE database."calorimetric_data_Approvals" DROP COLUMN "Standard_Section";

ALTER TABLE database."calorimetric_data_Approvals" DROP COLUMN "Standard_Standardizers";

ALTER TABLE database."calorimetric_data_Approvals" DROP COLUMN "Standard_Title";

ALTER TABLE database."calorimetric_data_Approvals" DROP COLUMN "Standard_Year";

DROP TYPE public.standardizer;
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'database') THEN
        CREATE SCHEMA database;
    END IF;
END $EF$;

CREATE EXTENSION IF NOT EXISTS pgcrypto;

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20240709144935_AddStatementToDataApproval';

COMMIT;

START TRANSACTION;

ALTER TABLE database."OpenIddictApplications" DROP COLUMN "ApplicationType";

ALTER TABLE database."OpenIddictApplications" DROP COLUMN "JsonWebKeySet";

ALTER TABLE database."OpenIddictApplications" DROP COLUMN "Settings";

ALTER TABLE database."OpenIddictApplications" RENAME COLUMN "ClientType" TO "Type";

ALTER TABLE database."user" ADD "Email" text NOT NULL DEFAULT '';

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20240522120225_UpgradeOpenIddict';

COMMIT;

START TRANSACTION;

DROP TABLE database."user";

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20230427133946_CreateUserTable';

COMMIT;

START TRANSACTION;

DROP TABLE database."OpenIddictScopes";

DROP TABLE database."OpenIddictTokens";

DROP TABLE database."OpenIddictAuthorizations";

DROP TABLE database."OpenIddictApplications";

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20230420143838_OpenIddict';

COMMIT;

