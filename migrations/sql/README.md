# Compulsory reasoning

## Initial entities

- Book — has a title, an ISBN, and a publication year
- Member — has a first name, a last name, and an email address
- Loan — records that a member has borrowed a book, with a loan date and an optional return date

### Notes

- ISBN and Email are unique
- `ReturnDate` is nullable — a `null` value means the book has not been returned yet
- Foreign keys on `Loans` prevent orphaned records if a book or member is deleted
- `HasDefaultValueSql("gen_random_uuid()")` is explicitly set so the database generates UUIDs, not EF Core — this ensures rows inserted outside of EF (e.g. raw SQL scripts run directly against Postgres) also get valid IDs

```csharp
builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
```

#### Running migrations

From the repo root:

```bash
.\scripts\New-Migration.ps1 -Name "MigrationName"
```

## Seed database

```sql
BEGIN;

INSERT INTO "Books" ("Id","Title","Isbn","PublishedYear","CreatedAt") VALUES
('3f2c9e05-6a1b-4d9a-ae3a-1a2b3c4d5e01','The Last Library','978-0143127741',2010,'2026-03-01 09:00:00+00'),
('b9f6c3e8-2c44-4bcb-9d88-2b3a4c5d6e02','Patterns in Code','978-0201633610',2000,'2026-03-01 09:05:00+00'),
('a1b2c3d4-e5f6-47a8-b9c0-d1e2f3a4b5c3','Practical Databases','978-1492040376',2018,'2026-03-01 09:10:00+00'),
('c2d3e4f5-1234-4abc-8def-0123456789ab','A History of Ideas','978-0307269997',2012,'2026-03-01 09:15:00+00'),
('d4e5f6a7-89ab-4123-9abc-2345678901cd','Modern Algorithms','978-0262033848',2021,'2026-03-01 09:20:00+00'),
('e7f8a9b0-2345-4def-8abc-3456789012ef','Designing Interfaces','978-1492051961',2019,'2026-03-01 09:25:00+00'),
('f0e1d2c3-4567-4aaa-9bbb-4567890123ab','Fictional Shores','978-0451524935',1995,'2026-03-01 09:30:00+00'),
('01234567-89ab-4cde-8f01-567890abcdef','Testing Strategies','978-0134277554',2016,'2026-03-01 09:35:00+00');

INSERT INTO "Members" ("Id","FirstName","LastName","Email","CreatedAt") VALUES
('11111111-2222-4333-8444-555555555555','Alice','Nguyen','alice.nguyen@example.com','2026-01-10 08:00:00+00'),
('22222222-3333-4444-8555-666666666666','Brian','Khan','brian.khan@example.com','2026-01-15 08:15:00+00'),
('33333333-4444-5555-8666-777777777777','Carla','Martinez','carla.martinez@example.com','2026-02-01 09:00:00+00'),
('44444444-5555-6666-8777-888888888888','Daniel','O''Brien','daniel.obrien@example.com','2026-02-05 10:00:00+00'),
('55555555-6666-7777-8888-999999999999','Elena','Fisher','elena.fisher@example.com','2026-02-20 11:00:00+00'),
('66666666-7777-8888-9999-aaaaaaaaaaaa','Faisal','Ali','faisal.ali@example.com','2026-02-22 12:00:00+00');

INSERT INTO "Loans" ("Id","BookId","MemberId","LoanDate","ReturnDate") VALUES
('9a1b2c3d-0001-4f6a-8b7c-000000000001','3f2c9e05-6a1b-4d9a-ae3a-1a2b3c4d5e01','11111111-2222-4333-8444-555555555555','2026-02-01','2026-02-10'),
('9a1b2c3d-0002-4f6a-8b7c-000000000002','b9f6c3e8-2c44-4bcb-9d88-2b3a4c5d6e02','22222222-3333-4444-8555-666666666666','2026-02-15',NULL),
('9a1b2c3d-0003-4f6a-8b7c-000000000003','a1b2c3d4-e5f6-47a8-b9c0-d1e2f3a4b5c3','11111111-2222-4333-8444-555555555555','2026-01-05','2026-01-20'),
('9a1b2c3d-0004-4f6a-8b7c-000000000004','c2d3e4f5-1234-4abc-8def-0123456789ab','33333333-4444-5555-8666-777777777777','2026-02-20',NULL),
('9a1b2c3d-0005-4f6a-8b7c-000000000005','d4e5f6a7-89ab-4123-9abc-2345678901cd','44444444-5555-6666-8777-888888888888','2025-12-15','2026-01-10'),
('9a1b2c3d-0006-4f6a-8b7c-000000000006','f0e1d2c3-4567-4aaa-9bbb-4567890123ab','55555555-6666-7777-8888-999999999999','2026-02-28',NULL),
('9a1b2c3d-0007-4f6a-8b7c-000000000007','01234567-89ab-4cde-8f01-567890abcdef','66666666-7777-8888-9999-aaaaaaaaaaaa','2026-02-10','2026-02-25');

COMMIT;
```

### Going forward I'll assume no down time is allowed

### Requirement 1: Books need Authors

- Add Author.cs
- Update Book.cs to have a Collection of Authors (EFCore creates many-to-many BookAuthor)
- AuthorConfiguration.cs uses .HasData to help with composite key creation in cases where books have no Author
- I run the below code once in program.cs to fix the old books, if I were to make this permenant it would be its own class using IHostedService

```cs
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LibrariumDbContext>();
    var booksWithNoAuthors = await context
        .Books.Include(b => b.Authors)
        .Where(b => !b.Authors.Any())
        .ToListAsync();
    if (booksWithNoAuthors.Any())
    {
        var unknownAuthor = await context.Authors.FindAsync(AuthorConfiguration.UnknownAuthorId);
        foreach (var book in booksWithNoAuthors)
            book.Authors.Add(unknownAuthor!);
        await context.SaveChangesAsync();
    }
}
```

- Update DTOs to something that made sense to me
- Update GetAll to include Authors
- Add proper validation for whats needed, but for the compulsory, nothing

### Requirement 2: Email addresses must be unique, and the member profile is expanding

- Emails are already unique in Emailconfiguration.cs (I didn't read ahead i just set that up by default woops lol)

```cs
builder.HasIndex(m => m.Email).IsUnique();
```

```sql
SELECT Email, COUNT(*)
FROM "Members"
GROUP BY Email
HAVING COUNT(*) > 1;

or to Shows member IDs or details

SELECT Email, COUNT(*) as Count, STRING_AGG(Id::text, ', ') as MemberIds
FROM "Members"
GROUP BY Email
HAVING COUNT(*) > 1;
```

- Then choose to contact the duplicates and resolve it manually, you can't just merge

- For adding phone numbers the column will not be NOT NULL instead the application layer will enforce
this for new members as well as sending out notifications to members to update their profile, at a later date
once confident the columns is populated in all rows it can be changed.

```sql
SELECT COUNT(*)
FROM "Members"
WHERE "PhoneNumber" IS NULL;
```

- Add this somewhere in the application code or fluent validation or .NET 10.0 added a new way to handle validation that I would look more into in this case

```cs
if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
    return Results.BadRequest("Phone number is required.");
```

### Requirement 3: Loans need a status and the existing client cannot be updated

- The frontend isnt ready for the changes so I wont make v2 yet until they are
- The new column will be nullable with a DB default of ```'Active'``` to protect loan inserts during the deployment window, when old backend instances are still running and don't know about ```Status``` yet
- Existing loans backfilled based on ReturnDate - Null means Active, not null means Returned
- Deploy

```sql
SELECT COUNT(*)
FROM "Loans"
WHERE "Status" IS NULL;
```

- Tighten column to NOT NULL later once backfill is confirmed
- existing endpoint ```status``` as an additive field that the front end will ignore until ready
- once the frontend is ready return date will be removed and v2 endpoint will be introduced
