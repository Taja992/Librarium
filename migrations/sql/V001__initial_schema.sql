CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260223133555_initial_schema') THEN
    CREATE TABLE "Books" (
        "Id" uuid NOT NULL DEFAULT (gen_random_uuid()),
        "Title" character varying(255) NOT NULL,
        "Isbn" character varying(20) NOT NULL,
        "PublishedYear" smallint NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (NOW()),
        CONSTRAINT "PK_Books" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260223133555_initial_schema') THEN
    CREATE TABLE "Members" (
        "Id" uuid NOT NULL DEFAULT (gen_random_uuid()),
        "FirstName" character varying(100) NOT NULL,
        "LastName" character varying(100) NOT NULL,
        "Email" character varying(255) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (NOW()),
        CONSTRAINT "PK_Members" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260223133555_initial_schema') THEN
    CREATE TABLE "Loans" (
        "Id" uuid NOT NULL DEFAULT (gen_random_uuid()),
        "BookId" uuid NOT NULL,
        "MemberId" uuid NOT NULL,
        "LoanDate" date NOT NULL DEFAULT (CURRENT_DATE),
        "ReturnDate" date,
        CONSTRAINT "PK_Loans" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Loans_Books_BookId" FOREIGN KEY ("BookId") REFERENCES "Books" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_Loans_Members_MemberId" FOREIGN KEY ("MemberId") REFERENCES "Members" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260223133555_initial_schema') THEN
    CREATE UNIQUE INDEX "IX_Books_Isbn" ON "Books" ("Isbn");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260223133555_initial_schema') THEN
    CREATE INDEX "IX_Loans_BookId" ON "Loans" ("BookId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260223133555_initial_schema') THEN
    CREATE INDEX "IX_Loans_MemberId" ON "Loans" ("MemberId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260223133555_initial_schema') THEN
    CREATE UNIQUE INDEX "IX_Members_Email" ON "Members" ("Email");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260223133555_initial_schema') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260223133555_initial_schema', '10.0.3');
    END IF;
END $EF$;
COMMIT;

