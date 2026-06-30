START TRANSACTION;

CREATE TABLE database."OpenIddictApplications" (
    "Id" text NOT NULL,
    "ClientId" character varying(100),
    "ClientSecret" text,
    "ConcurrencyToken" character varying(50),
    "ConsentType" character varying(50),
    "DisplayName" text,
    "DisplayNames" text,
    "Permissions" text,
    "PostLogoutRedirectUris" text,
    "Properties" text,
    "RedirectUris" text,
    "Requirements" text,
    "Type" character varying(50),
    CONSTRAINT "PK_OpenIddictApplications" PRIMARY KEY ("Id")
);

CREATE TABLE database."OpenIddictScopes" (
    "Id" text NOT NULL,
    "ConcurrencyToken" character varying(50),
    "Description" text,
    "Descriptions" text,
    "DisplayName" text,
    "DisplayNames" text,
    "Name" character varying(200),
    "Properties" text,
    "Resources" text,
    CONSTRAINT "PK_OpenIddictScopes" PRIMARY KEY ("Id")
);

CREATE TABLE database."OpenIddictAuthorizations" (
    "Id" text NOT NULL,
    "ApplicationId" text,
    "ConcurrencyToken" character varying(50),
    "CreationDate" timestamp with time zone,
    "Properties" text,
    "Scopes" text,
    "Status" character varying(50),
    "Subject" character varying(400),
    "Type" character varying(50),
    CONSTRAINT "PK_OpenIddictAuthorizations" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_OpenIddictAuthorizations_OpenIddictApplications_Application~" FOREIGN KEY ("ApplicationId") REFERENCES database."OpenIddictApplications" ("Id")
);

CREATE TABLE database."OpenIddictTokens" (
    "Id" text NOT NULL,
    "ApplicationId" text,
    "AuthorizationId" text,
    "ConcurrencyToken" character varying(50),
    "CreationDate" timestamp with time zone,
    "ExpirationDate" timestamp with time zone,
    "Payload" text,
    "Properties" text,
    "RedemptionDate" timestamp with time zone,
    "ReferenceId" character varying(100),
    "Status" character varying(50),
    "Subject" character varying(400),
    "Type" character varying(50),
    CONSTRAINT "PK_OpenIddictTokens" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_OpenIddictTokens_OpenIddictApplications_ApplicationId" FOREIGN KEY ("ApplicationId") REFERENCES database."OpenIddictApplications" ("Id"),
    CONSTRAINT "FK_OpenIddictTokens_OpenIddictAuthorizations_AuthorizationId" FOREIGN KEY ("AuthorizationId") REFERENCES database."OpenIddictAuthorizations" ("Id")
);

CREATE UNIQUE INDEX "IX_OpenIddictApplications_ClientId" ON database."OpenIddictApplications" ("ClientId");

CREATE INDEX "IX_OpenIddictAuthorizations_ApplicationId_Status_Subject_Type" ON database."OpenIddictAuthorizations" ("ApplicationId", "Status", "Subject", "Type");

CREATE UNIQUE INDEX "IX_OpenIddictScopes_Name" ON database."OpenIddictScopes" ("Name");

CREATE INDEX "IX_OpenIddictTokens_ApplicationId_Status_Subject_Type" ON database."OpenIddictTokens" ("ApplicationId", "Status", "Subject", "Type");

CREATE INDEX "IX_OpenIddictTokens_AuthorizationId" ON database."OpenIddictTokens" ("AuthorizationId");

CREATE UNIQUE INDEX "IX_OpenIddictTokens_ReferenceId" ON database."OpenIddictTokens" ("ReferenceId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20230420143838_OpenIddict', '8.0.6');

COMMIT;

START TRANSACTION;

CREATE TABLE database."user" (
    "Id" uuid NOT NULL DEFAULT (gen_random_uuid()),
    "Subject" text NOT NULL,
    "Email" text NOT NULL,
    "Name" text NOT NULL,
    CONSTRAINT "PK_user" PRIMARY KEY ("Id")
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20230427133946_CreateUserTable', '8.0.6');

COMMIT;

START TRANSACTION;

ALTER TABLE database."user" DROP COLUMN "Email";

ALTER TABLE database."OpenIddictApplications" RENAME COLUMN "Type" TO "ClientType";

ALTER TABLE database."OpenIddictApplications" ADD "ApplicationType" character varying(50);

ALTER TABLE database."OpenIddictApplications" ADD "JsonWebKeySet" text;

ALTER TABLE database."OpenIddictApplications" ADD "Settings" text;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240522120225_UpgradeOpenIddict', '8.0.6');

COMMIT;

START TRANSACTION;

CREATE TYPE public.standardizer AS ENUM ('aerc', 'agi', 'ashrae', 'breeam', 'bs', 'bsi', 'cen', 'cie', 'dgnb', 'din', 'dvwg', 'iec', 'ies', 'ift', 'iso', 'jis', 'leed', 'nfrc', 'riba', 'ul', 'unece', 'vdi', 'vff', 'well');
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'database') THEN
        CREATE SCHEMA database;
    END IF;
END $EF$;

CREATE EXTENSION IF NOT EXISTS pgcrypto;

ALTER TABLE database."photovoltaic_data_Approvals" ADD "Publication_Abstract" text;

ALTER TABLE database."photovoltaic_data_Approvals" ADD "Publication_ArXiv" text;

ALTER TABLE database."photovoltaic_data_Approvals" ADD "Publication_Authors" text[];

ALTER TABLE database."photovoltaic_data_Approvals" ADD "Publication_Doi" text;

ALTER TABLE database."photovoltaic_data_Approvals" ADD "Publication_Section" text;

ALTER TABLE database."photovoltaic_data_Approvals" ADD "Publication_Title" text;

ALTER TABLE database."photovoltaic_data_Approvals" ADD "Publication_Urn" text;

ALTER TABLE database."photovoltaic_data_Approvals" ADD "Publication_WebAddress" text;

ALTER TABLE database."photovoltaic_data_Approvals" ADD "Standard_Abstract" text;

ALTER TABLE database."photovoltaic_data_Approvals" ADD "Standard_Locator" text;

ALTER TABLE database."photovoltaic_data_Approvals" ADD "Standard_Numeration_MainNumber" text;

ALTER TABLE database."photovoltaic_data_Approvals" ADD "Standard_Numeration_Prefix" text;

ALTER TABLE database."photovoltaic_data_Approvals" ADD "Standard_Numeration_Suffix" text;

ALTER TABLE database."photovoltaic_data_Approvals" ADD "Standard_Section" text;

ALTER TABLE database."photovoltaic_data_Approvals" ADD "Standard_Standardizers" standardizer[];

ALTER TABLE database."photovoltaic_data_Approvals" ADD "Standard_Title" text;

ALTER TABLE database."photovoltaic_data_Approvals" ADD "Standard_Year" integer;

ALTER TABLE database."optical_data_Approvals" ADD "Publication_Abstract" text;

ALTER TABLE database."optical_data_Approvals" ADD "Publication_ArXiv" text;

ALTER TABLE database."optical_data_Approvals" ADD "Publication_Authors" text[];

ALTER TABLE database."optical_data_Approvals" ADD "Publication_Doi" text;

ALTER TABLE database."optical_data_Approvals" ADD "Publication_Section" text;

ALTER TABLE database."optical_data_Approvals" ADD "Publication_Title" text;

ALTER TABLE database."optical_data_Approvals" ADD "Publication_Urn" text;

ALTER TABLE database."optical_data_Approvals" ADD "Publication_WebAddress" text;

ALTER TABLE database."optical_data_Approvals" ADD "Standard_Abstract" text;

ALTER TABLE database."optical_data_Approvals" ADD "Standard_Locator" text;

ALTER TABLE database."optical_data_Approvals" ADD "Standard_Numeration_MainNumber" text;

ALTER TABLE database."optical_data_Approvals" ADD "Standard_Numeration_Prefix" text;

ALTER TABLE database."optical_data_Approvals" ADD "Standard_Numeration_Suffix" text;

ALTER TABLE database."optical_data_Approvals" ADD "Standard_Section" text;

ALTER TABLE database."optical_data_Approvals" ADD "Standard_Standardizers" standardizer[];

ALTER TABLE database."optical_data_Approvals" ADD "Standard_Title" text;

ALTER TABLE database."optical_data_Approvals" ADD "Standard_Year" integer;

ALTER TABLE database."hygrothermal_data_Approvals" ADD "Publication_Abstract" text;

ALTER TABLE database."hygrothermal_data_Approvals" ADD "Publication_ArXiv" text;

ALTER TABLE database."hygrothermal_data_Approvals" ADD "Publication_Authors" text[];

ALTER TABLE database."hygrothermal_data_Approvals" ADD "Publication_Doi" text;

ALTER TABLE database."hygrothermal_data_Approvals" ADD "Publication_Section" text;

ALTER TABLE database."hygrothermal_data_Approvals" ADD "Publication_Title" text;

ALTER TABLE database."hygrothermal_data_Approvals" ADD "Publication_Urn" text;

ALTER TABLE database."hygrothermal_data_Approvals" ADD "Publication_WebAddress" text;

ALTER TABLE database."hygrothermal_data_Approvals" ADD "Standard_Abstract" text;

ALTER TABLE database."hygrothermal_data_Approvals" ADD "Standard_Locator" text;

ALTER TABLE database."hygrothermal_data_Approvals" ADD "Standard_Numeration_MainNumber" text;

ALTER TABLE database."hygrothermal_data_Approvals" ADD "Standard_Numeration_Prefix" text;

ALTER TABLE database."hygrothermal_data_Approvals" ADD "Standard_Numeration_Suffix" text;

ALTER TABLE database."hygrothermal_data_Approvals" ADD "Standard_Section" text;

ALTER TABLE database."hygrothermal_data_Approvals" ADD "Standard_Standardizers" standardizer[];

ALTER TABLE database."hygrothermal_data_Approvals" ADD "Standard_Title" text;

ALTER TABLE database."hygrothermal_data_Approvals" ADD "Standard_Year" integer;

ALTER TABLE database."calorimetric_data_Approvals" ADD "Publication_Abstract" text;

ALTER TABLE database."calorimetric_data_Approvals" ADD "Publication_ArXiv" text;

ALTER TABLE database."calorimetric_data_Approvals" ADD "Publication_Authors" text[];

ALTER TABLE database."calorimetric_data_Approvals" ADD "Publication_Doi" text;

ALTER TABLE database."calorimetric_data_Approvals" ADD "Publication_Section" text;

ALTER TABLE database."calorimetric_data_Approvals" ADD "Publication_Title" text;

ALTER TABLE database."calorimetric_data_Approvals" ADD "Publication_Urn" text;

ALTER TABLE database."calorimetric_data_Approvals" ADD "Publication_WebAddress" text;

ALTER TABLE database."calorimetric_data_Approvals" ADD "Standard_Abstract" text;

ALTER TABLE database."calorimetric_data_Approvals" ADD "Standard_Locator" text;

ALTER TABLE database."calorimetric_data_Approvals" ADD "Standard_Numeration_MainNumber" text;

ALTER TABLE database."calorimetric_data_Approvals" ADD "Standard_Numeration_Prefix" text;

ALTER TABLE database."calorimetric_data_Approvals" ADD "Standard_Numeration_Suffix" text;

ALTER TABLE database."calorimetric_data_Approvals" ADD "Standard_Section" text;

ALTER TABLE database."calorimetric_data_Approvals" ADD "Standard_Standardizers" standardizer[];

ALTER TABLE database."calorimetric_data_Approvals" ADD "Standard_Title" text;

ALTER TABLE database."calorimetric_data_Approvals" ADD "Standard_Year" integer;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240709144935_AddStatementToDataApproval', '8.0.6');

COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'database') THEN
        CREATE SCHEMA database;
    END IF;
END $EF$;

CREATE TYPE database.data_kind AS ENUM ('calorimetric_data', 'hygrothermal_data', 'optical_data', 'photovoltaic_data');
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'database') THEN
        CREATE SCHEMA database;
    END IF;
END $EF$;

CREATE TYPE database.standardizer AS ENUM ('aerc', 'agi', 'ashrae', 'breeam', 'bs', 'bsi', 'cen', 'cie', 'dgnb', 'din', 'dvwg', 'iec', 'ies', 'ift', 'iso', 'jis', 'leed', 'nfrc', 'riba', 'ul', 'unece', 'vdi', 'vff', 'well');

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'database') THEN
        CREATE SCHEMA database;
    END IF;
END $EF$;

CREATE EXTENSION IF NOT EXISTS pgcrypto;

ALTER TABLE database."photovoltaic_data_Sources" ALTER COLUMN "Value_DataKind" TYPE database.data_kind USING "Value_DataKind"::text::database.data_kind;

ALTER TABLE database."photovoltaic_data_Approvals" ALTER COLUMN "Standard_Standardizers" TYPE database.standardizer[] USING "Standard_Standardizers"::text::database.standardizer[];

ALTER TABLE database."optical_data_Sources" ALTER COLUMN "Value_DataKind" TYPE database.data_kind USING "Value_DataKind"::text::database.data_kind;

ALTER TABLE database."optical_data_Approvals" ALTER COLUMN "Standard_Standardizers" TYPE database.standardizer[] USING "Standard_Standardizers"::text::database.standardizer[];

ALTER TABLE database."hygrothermal_data_Sources" ALTER COLUMN "Value_DataKind" TYPE database.data_kind USING "Value_DataKind"::text::database.data_kind;

ALTER TABLE database."hygrothermal_data_Approvals" ALTER COLUMN "Standard_Standardizers" TYPE database.standardizer[] USING "Standard_Standardizers"::text::database.standardizer[];

ALTER TABLE database."calorimetric_data_Sources" ALTER COLUMN "Value_DataKind" TYPE database.data_kind USING "Value_DataKind"::text::database.data_kind;

ALTER TABLE database."calorimetric_data_Approvals" ALTER COLUMN "Standard_Standardizers" TYPE database.standardizer[] USING "Standard_Standardizers"::text::database.standardizer[];

DROP TYPE public.data_kind;
DROP TYPE public.standardizer;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240722133619_MoveEnumTypesToMetabaseSchema', '8.0.6');

COMMIT;

