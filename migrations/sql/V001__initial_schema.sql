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

-- Seed initial data: I did this manually
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260223133555_initial_schema') THEN
    INSERT INTO "Books" ("Id", "Title", "Isbn", "PublishedYear", "CreatedAt") VALUES
    ('3f2c9e05-6a1b-4d9a-ae3a-1a2b3c4d5e01', 'The Last Library',    '978-0143127741', 2010, '2026-03-01 09:00:00+00'),
    ('b9f6c3e8-2c44-4bcb-9d88-2b3a4c5d6e02', 'Patterns in Code',    '978-0201633610', 2000, '2026-03-01 09:05:00+00'),
    ('a1b2c3d4-e5f6-47a8-b9c0-d1e2f3a4b5c3', 'Practical Databases', '978-1492040376', 2018, '2026-03-01 09:10:00+00'),
    ('c2d3e4f5-1234-4abc-8def-0123456789ab', 'A History of Ideas',  '978-0307269997', 2012, '2026-03-01 09:15:00+00'),
    ('d4e5f6a7-89ab-4123-9abc-2345678901cd', 'Modern Algorithms',   '978-0262033848', 2021, '2026-03-01 09:20:00+00'),
    ('e7f8a9b0-2345-4def-8abc-3456789012ef', 'Designing Interfaces','978-1492051961', 2019, '2026-03-01 09:25:00+00'),
    ('f0e1d2c3-4567-4aaa-9bbb-4567890123ab', 'Fictional Shores',    '978-0451524935', 1995, '2026-03-01 09:30:00+00'),
    ('01234567-89ab-4cde-8f01-567890abcdef', 'Testing Strategies',  '978-0134277554', 2016, '2026-03-01 09:35:00+00');

    INSERT INTO "Members" ("Id", "FirstName", "LastName", "Email", "CreatedAt") VALUES
    ('11111111-2222-4333-8444-555555555555', 'Alice',  'Nguyen',   'alice.nguyen@example.com',   '2026-01-10 08:00:00+00'),
    ('22222222-3333-4444-8555-666666666666', 'Brian',  'Khan',     'brian.khan@example.com',     '2026-01-15 08:15:00+00'),
    ('33333333-4444-5555-8666-777777777777', 'Carla',  'Martinez', 'carla.martinez@example.com', '2026-02-01 09:00:00+00'),
    ('44444444-5555-6666-8777-888888888888', 'Daniel', 'O''Brien', 'daniel.obrien@example.com',  '2026-02-05 10:00:00+00'),
    ('55555555-6666-7777-8888-999999999999', 'Elena',  'Fisher',   'elena.fisher@example.com',   '2026-02-20 11:00:00+00'),
    ('66666666-7777-8888-9999-aaaaaaaaaaaa', 'Faisal', 'Ali',      'faisal.ali@example.com',     '2026-02-22 12:00:00+00');

    INSERT INTO "Loans" ("Id", "BookId", "MemberId", "LoanDate", "ReturnDate") VALUES
    ('9a1b2c3d-0001-4f6a-8b7c-000000000001', '3f2c9e05-6a1b-4d9a-ae3a-1a2b3c4d5e01', '11111111-2222-4333-8444-555555555555', '2026-02-01', '2026-02-10'),
    ('9a1b2c3d-0002-4f6a-8b7c-000000000002', 'b9f6c3e8-2c44-4bcb-9d88-2b3a4c5d6e02', '22222222-3333-4444-8555-666666666666', '2026-02-15', NULL),
    ('9a1b2c3d-0003-4f6a-8b7c-000000000003', 'a1b2c3d4-e5f6-47a8-b9c0-d1e2f3a4b5c3', '11111111-2222-4333-8444-555555555555', '2026-01-05', '2026-01-20'),
    ('9a1b2c3d-0004-4f6a-8b7c-000000000004', 'c2d3e4f5-1234-4abc-8def-0123456789ab', '33333333-4444-5555-8666-777777777777', '2026-02-20', NULL),
    ('9a1b2c3d-0005-4f6a-8b7c-000000000005', 'd4e5f6a7-89ab-4123-9abc-2345678901cd', '44444444-5555-6666-8777-888888888888', '2025-12-15', '2026-01-10'),
    ('9a1b2c3d-0006-4f6a-8b7c-000000000006', 'f0e1d2c3-4567-4aaa-9bbb-4567890123ab', '55555555-6666-7777-8888-999999999999', '2026-02-28', NULL),
    ('9a1b2c3d-0007-4f6a-8b7c-000000000007', '01234567-89ab-4cde-8f01-567890abcdef', '66666666-7777-8888-9999-aaaaaaaaaaaa', '2026-02-10', '2026-02-25');
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

