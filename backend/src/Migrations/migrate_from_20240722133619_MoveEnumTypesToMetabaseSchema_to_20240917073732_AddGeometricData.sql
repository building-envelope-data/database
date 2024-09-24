﻿START TRANSACTION;

ALTER TYPE database.data_kind ADD VALUE 'geometric_data';

ALTER TABLE database.get_https_resource ADD "GeometricDataId" uuid;

CREATE TABLE database.geometric_data (
    "Id" uuid NOT NULL DEFAULT (gen_random_uuid()),
    "Thicknesses" double precision[] NOT NULL,
    "Locale" text NOT NULL,
    "ComponentId" uuid NOT NULL,
    "Name" text,
    "Description" text,
    "Warnings" text[] NOT NULL,
    "CreatorId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "AppliedMethod_MethodId" uuid NOT NULL,
    CONSTRAINT "PK_geometric_data" PRIMARY KEY ("Id")
);

CREATE TABLE database."geometric_data_Approvals" (
    "GeometricDataId" uuid NOT NULL,
    "Id" integer GENERATED BY DEFAULT AS IDENTITY,
    "ApproverId" uuid NOT NULL,
    "Timestamp" timestamp with time zone NOT NULL,
    "Signature" text NOT NULL,
    "KeyFingerprint" text NOT NULL,
    "Query" text NOT NULL,
    "Response" text NOT NULL,
    "Publication_Authors" text[],
    "Publication_Doi" text,
    "Publication_ArXiv" text,
    "Publication_Urn" text,
    "Publication_WebAddress" text,
    "Publication_Title" text,
    "Publication_Abstract" text,
    "Publication_Section" text,
    "Standard_Year" integer,
    "Standard_Numeration_Prefix" text,
    "Standard_Numeration_MainNumber" text,
    "Standard_Numeration_Suffix" text,
    "Standard_Standardizers" standardizer[],
    "Standard_Locator" text,
    "Standard_Title" text,
    "Standard_Abstract" text,
    "Standard_Section" text,
    CONSTRAINT "PK_geometric_data_Approvals" PRIMARY KEY ("GeometricDataId", "Id"),
    CONSTRAINT "FK_geometric_data_Approvals_geometric_data_GeometricDataId" FOREIGN KEY ("GeometricDataId") REFERENCES database.geometric_data ("Id") ON DELETE CASCADE
);

CREATE TABLE database."geometric_data_Arguments" (
    "AppliedMethodGeometricDataId" uuid NOT NULL,
    "Id" integer GENERATED BY DEFAULT AS IDENTITY,
    "Name" text NOT NULL,
    "Value" jsonb NOT NULL,
    CONSTRAINT "PK_geometric_data_Arguments" PRIMARY KEY ("AppliedMethodGeometricDataId", "Id"),
    CONSTRAINT "FK_geometric_data_Arguments_geometric_data_AppliedMethodGeomet~" FOREIGN KEY ("AppliedMethodGeometricDataId") REFERENCES database.geometric_data ("Id") ON DELETE CASCADE
);

CREATE TABLE database."geometric_data_Sources" (
    "AppliedMethodGeometricDataId" uuid NOT NULL,
    "Id" integer GENERATED BY DEFAULT AS IDENTITY,
    "Name" text NOT NULL,
    "Value_DataId" uuid NOT NULL,
    "Value_DataTimestamp" timestamp with time zone NOT NULL,
    "Value_DataKind" data_kind NOT NULL,
    "Value_DatabaseId" uuid NOT NULL,
    CONSTRAINT "PK_geometric_data_Sources" PRIMARY KEY ("AppliedMethodGeometricDataId", "Id"),
    CONSTRAINT "FK_geometric_data_Sources_geometric_data_AppliedMethodGeometri~" FOREIGN KEY ("AppliedMethodGeometricDataId") REFERENCES database.geometric_data ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_get_https_resource_GeometricDataId" ON database.get_https_resource ("GeometricDataId");

ALTER TABLE database.get_https_resource ADD CONSTRAINT "FK_get_https_resource_geometric_data_GeometricDataId" FOREIGN KEY ("GeometricDataId") REFERENCES database.geometric_data ("Id");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240917073732_AddGeometricData', '8.0.8');

COMMIT;

