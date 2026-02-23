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
