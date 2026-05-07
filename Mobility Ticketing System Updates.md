# Code Fix Snippets - Mobility Ticketing Review

---

## Issue 1 [HIGH] - No Transaction in Ticket Purchase

**Before** (`TicketPurchaseService.cs`): three unguarded sequential awaits, each on a separate connection.

```csharp
// Charge happens here - external, cannot be rolled back
var charge = await paymentGateway.ChargeAsync(...);

// Three separate connections - no atomicity
await tickets.SaveAsync(ticket, cancellationToken);
await payments.SaveAsync(payment, cancellationToken);
await trips.IncrementReservedSeatsAsync(trip.Id, 1, cancellationToken);
```

**After**: payment gateway call stays outside; all three DB writes share one transaction.

```csharp
// External call first - outside any transaction
var charge = await paymentGateway.ChargeAsync(
    command.UserId, command.PaymentToken, amount, command.Currency, cancellationToken);

if (!charge.Status.Equals("Captured", StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("Payment failed.");

// Single transaction covers all three writes
await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
try
{
    await tickets.SaveAsync(ticket, connection, transaction, cancellationToken);
    await payments.SaveAsync(payment, connection, transaction, cancellationToken);
    await trips.IncrementReservedSeatsAsync(trip.Id, 1, connection, transaction, cancellationToken);
    await transaction.CommitAsync(cancellationToken);
}
catch
{
    await transaction.RollbackAsync(cancellationToken);
    throw;
}
```

---

## Issue 2 [HIGH] - Client-Controlled Price Accepted as Payment Amount

**Before** (`TicketPurchaseService.cs`): client can pass any price, including zero.

```csharp
var amount = command.ClientQuotedPrice ?? product.Price;
```

**After**: price is always derived from the server-side product record.

```csharp
var amount = product.Price;
```

`ClientQuotedPrice` is also removed from `PurchaseTicketCommand`:

```csharp
// Before
public record PurchaseTicketCommand(
    string UserId,
    string TripId,
    string ProductCode,
    string PaymentToken,
    string Currency,
    double? ClientQuotedPrice);   // <-- remove this

// After
public record PurchaseTicketCommand(
    string UserId,
    string TripId,
    string ProductCode,
    string PaymentToken,
    string Currency);
```

---

## Issue 3 [HIGH] - Ticket Code Collisions Under Concurrency

**Before** - schema has no uniqueness guarantee (`001_initial_schema.sql`):

```sql
ticket_code text not null   -- no UNIQUE constraint
```

Generator uses second-precision timestamps (`TicketPurchaseService.cs`):

```csharp
private static string CreateTicketCode(string userId, DateTime now)
    => $"{userId[..Math.Min(6, userId.Length)]}-{now:yyyyMMddHHmmss}";
// Two purchases by the same user in the same second → identical code
```

**After** - add uniqueness at the database level:

```sql
ALTER TABLE tickets
    ADD CONSTRAINT uq_ticket_code UNIQUE (ticket_code);
```

Replace the timestamp suffix with a cryptographically random component:

```csharp
private static string CreateTicketCode()
    => Guid.NewGuid().ToString("N");   // 32 random hex chars, collision-free
```

---

## Issue 4 [HIGH] - Double-Spend Race in Ticket Validation

**Before** (`TicketValidationService.cs`): plain `SELECT` with no lock, then two separate writes. Concurrent validators both pass the status check.

```csharp
// Step 1 - unguarded read, no FOR UPDATE
var ticket = await tickets.GetByCodeAsync(command.TicketCode, cancellationToken);

if (ticket.Status != "Paid") return Reject(...);   // both concurrent callers pass this

// Step 2 - separate write (connection #1)
await tickets.AddValidationAsync(validation, cancellationToken);

// Step 3 - separate write (connection #2), too late to prevent the race
await tickets.UpdateStatusAsync(ticket.Id, "Validated", cancellationToken);
```

**After**: single atomic UPDATE that only succeeds for one caller; insert validation in the same transaction.

```sql
-- Returns the row only when this caller wins the race
UPDATE tickets
SET    status = 'Validated'
WHERE  id     = @ticketId
  AND  status = 'Paid'
RETURNING id;
```

```csharp
await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

// Atomic status flip - exactly one concurrent caller will get a row back
var claimedId = await connection.QuerySingleOrDefaultAsync<string>(
    "UPDATE tickets SET status = 'Validated' WHERE id = @ticketId AND status = 'Paid' RETURNING id",
    new { ticketId = ticket.Id }, transaction);

if (claimedId is null)
    return new TicketValidationResult(ticket.Id, false, "Already validated", now);

await connection.ExecuteAsync(
    "INSERT INTO validations (...) VALUES (...)",
    validation, transaction);

await transaction.CommitAsync(cancellationToken);
```

---

## Issue 5 [HIGH] - Timetable Replacement Not Transactional; Orphans Sold Tickets

**Before** (`PostgresTripRepository.cs`): delete then loop-insert, no transaction; no FK on `tickets.trip_id`.

```csharp
// No transaction - mid-loop failure leaves zero trips for the route
await connection.ExecuteAsync(
    "DELETE FROM trips WHERE route_id = @routeId", new { routeId });

foreach (var trip in trips)
{
    await connection.ExecuteAsync("INSERT INTO trips (...) VALUES (...)", trip);
}
```

**After**: pre-delete guard + single transaction + FK migration.

```csharp
await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

// Guard: refuse if any sold ticket references a trip being removed
var activeTripIds = existingTrips.Select(t => t.Id).ToArray();
var blocked = await connection.QueryFirstOrDefaultAsync<string>(
    "SELECT trip_id FROM tickets WHERE trip_id = ANY(@ids) AND status NOT IN ('Cancelled','Refunded') LIMIT 1",
    new { ids = activeTripIds }, transaction);

if (blocked is not null)
    throw new InvalidOperationException($"Cannot replace timetable: active tickets exist for trip {blocked}.");

await connection.ExecuteAsync(
    "DELETE FROM trips WHERE route_id = @routeId", new { routeId }, transaction);

foreach (var trip in trips)
    await connection.ExecuteAsync("INSERT INTO trips (...) VALUES (...)", trip, transaction);

await transaction.CommitAsync(cancellationToken);
```

Migration to add the missing foreign key:

```sql
-- 005_add_trip_fk.sql
ALTER TABLE tickets
    ADD CONSTRAINT fk_tickets_trip
    FOREIGN KEY (trip_id) REFERENCES trips(id)
    ON DELETE RESTRICT;
```

---

## Issue 6 [MEDIUM] - Broken CQRS Projection

**Before** (`MongoJourneySearchReadStore.cs`): writes only the route ID string into `RouteName`; all other fields left unchanged or empty.

```csharp
await _collection.UpdateOneAsync(
    Builders<JourneySearchDocument>.Filter.Eq(x => x.RouteId, routeId),
    Builders<JourneySearchDocument>.Update.Set(x => x.RouteName, routeId), // writes ID into name field
    new UpdateOptions { IsUpsert = true },
    cancellationToken);
// No departure list, no availability, no price - document is useless
```

**After**: query PostgreSQL for the full route snapshot and replace the document.

```csharp
public async Task UpsertRouteAsync(string routeId, CancellationToken cancellationToken = default)
{
    // Fetch real data from PostgreSQL
    var route   = await _postgresRoutes.GetByIdAsync(routeId, cancellationToken);
    var trips   = await _postgresTrips.GetUpcomingTripsForRouteAsync(routeId, DateTime.UtcNow, cancellationToken);
    var product = await _postgresProducts.GetActiveProductAsync(route.CityId, cancellationToken);

    var document = new JourneySearchDocument
    {
        Id        = $"{route.CityId}:{routeId}",
        CityId    = route.CityId,
        RouteId   = route.Id,
        RouteName = route.DisplayName,          // actual name, not the ID
        Mode      = route.Mode,
        Price     = product.Price,
        Currency  = product.Currency,
        Departures = trips.Select(t => new DepartureEntry
        {
            TripId         = t.Id,
            DepartureUtc   = t.ScheduledDepartureUtc,
            ArrivalUtc     = t.ScheduledArrivalUtc,
            AvailableSeats = t.Capacity - t.ReservedSeats   // live availability
        }).ToList()
    };

    await _collection.ReplaceOneAsync(
        Builders<JourneySearchDocument>.Filter.Eq(x => x.RouteId, routeId),
        document,
        new ReplaceOptions { IsUpsert = true },
        cancellationToken);
}
```

---

## Issue 7 [MEDIUM] - Non-Atomic Availability Check; Unbucketed Cache Key

**Before** - read-modify-write on the Redis counter (not atomic):

```csharp
// TicketPurchaseService.cs
var remaining = await availability.GetRemainingSeatsAsync(trip.RouteId, trip.ServiceDate, cancellationToken);
if (remaining is not null && remaining <= 0)
    throw new InvalidOperationException("No remaining capacity.");
// ... payment and writes ...
if (remaining.HasValue)
    await availability.SetRemainingSeatsAsync(
        trip.RouteId, trip.ServiceDate, remaining.Value - 1, cancellationToken);
// Two concurrent purchases both read the same value and both write "remaining - 1"
```

**After**: atomic `DECRBY`; seed from PostgreSQL on cache miss; reject if the decrement goes negative.

```csharp
// Seed from PostgreSQL if the key is absent
var remaining = await availability.GetRemainingSeatsAsync(trip.RouteId, trip.ServiceDate, cancellationToken);
if (remaining is null)
{
    var seats = trip.Capacity - trip.ReservedSeats;
    await availability.SetRemainingSeatsAsync(trip.RouteId, trip.ServiceDate, seats, cancellationToken);
}

// Atomic decrement - safe under concurrency
var afterDecrement = await availability.DecrementRemainingSeatsAsync(
    trip.RouteId, trip.ServiceDate, 1, cancellationToken);

if (afterDecrement < 0)
{
    // Restore and reject - seat was already taken
    await availability.DecrementRemainingSeatsAsync(trip.RouteId, trip.ServiceDate, -1, cancellationToken);
    throw new InvalidOperationException("Trip has no remaining capacity.");
}
```

**Before** - search cache key ignores departure time (`RedisSearchCache.cs`):

```csharp
private static RedisKey Key(JourneySearchRequest request)
    => $"search:{request.CityId}:{request.FromStopId}:{request.ToStopId}";
// A 07:00 search and an 18:00 search for the same route share one cached result
```

**After**: bucket by date and 2-hour window to scope results correctly.

```csharp
private static RedisKey Key(JourneySearchRequest request)
{
    var hourBucket = request.DepartureAfterUtc.Hour / 2 * 2;   // e.g. 06, 08, 10 …
    return $"search:{request.CityId}:{request.FromStopId}:{request.ToStopId}" +
           $":{request.DepartureAfterUtc:yyyyMMdd}:{hourBucket:D2}";
}
```

---

## Issue 8 [MEDIUM] - Reporting Inaccuracies

**Before** - trigger fires on every payment insert regardless of status (`002_reporting_views.sql`):

```sql
create or replace function apply_payment_to_daily_revenue()
returns trigger language plpgsql as $$
begin
    insert into daily_revenue_by_operator (...)
    select r.operator_id, cast(new.created_utc as date), new.currency, new.amount, 1
    from tickets t
    join trips tr on tr.id = t.trip_id
    join routes r  on r.id  = tr.route_id
    where t.id = new.ticket_id
    on conflict (...) do update set
        gross_amount = daily_revenue_by_operator.gross_amount + excluded.gross_amount,
        ticket_count = daily_revenue_by_operator.ticket_count + 1;
    return new;
end;
$$;
-- Fires for Failed and Pending payments too - inflates revenue figures
```

**After**: guard on payment status before inserting.

```sql
create or replace function apply_payment_to_daily_revenue()
returns trigger language plpgsql as $$
begin
    if new.status <> 'Captured' then   -- only count successful payments
        return new;
    end if;

    insert into daily_revenue_by_operator (operator_id, service_date, currency, gross_amount, ticket_count)
    select r.operator_id, cast(new.created_utc as date), new.currency, new.amount, 1
    from tickets t
    join trips  tr on tr.id = t.trip_id
    join routes r  on r.id  = tr.route_id
    where t.id = new.ticket_id
    on conflict (operator_id, service_date, currency) do update set
        gross_amount = daily_revenue_by_operator.gross_amount + excluded.gross_amount,
        ticket_count = daily_revenue_by_operator.ticket_count + 1;

    return new;
end;
$$;
```

**Before** - function-wrapped predicate blocks the index (`PostgresReportingRepository.cs`):

```sql
where to_char(p.created_utc, 'YYYY-MM') = @month
-- idx_payments_created_utc cannot be used as a range scan
```

**After**: range predicate that the planner can satisfy with the existing index.

```sql
where p.created_utc >= @monthStart       -- e.g. 2026-05-01 00:00:00
  and p.created_utc <  @monthEnd         -- e.g. 2026-06-01 00:00:00
  and p.status = 'Captured'
```

**Before** - monetary columns use floating-point (`001_initial_schema.sql`):

```sql
price  double precision not null,   -- tickets, products
amount double precision not null    -- payments
```

**After**: migration to exact decimal type.

```sql
-- 006_monetary_precision.sql
ALTER TABLE tickets  ALTER COLUMN price  TYPE numeric(12,2);
ALTER TABLE products ALTER COLUMN price  TYPE numeric(12,2);
ALTER TABLE payments ALTER COLUMN amount TYPE numeric(12,2);
ALTER TABLE daily_revenue_by_operator ALTER COLUMN gross_amount TYPE numeric(14,2);
```

---

## Issue 9 [MEDIUM] - Missing Referential Integrity and Status Constraints

**Before** - six FK relationships are plain `text` columns; status values are unconstrained (`001_initial_schema.sql`):

```sql
-- No FK - orphaned tickets go undetected
trip_id text,

-- No constraint - any string is a valid status
status text not null
```

**After** - migration adding FKs and CHECK constraints:

```sql
-- 007_add_constraints.sql

-- Foreign keys with RESTRICT to protect financial records
ALTER TABLE tickets
    ADD CONSTRAINT fk_tickets_user    FOREIGN KEY (user_id)  REFERENCES users(id)     ON DELETE RESTRICT,
    ADD CONSTRAINT fk_tickets_trip    FOREIGN KEY (trip_id)  REFERENCES trips(id)     ON DELETE RESTRICT;

ALTER TABLE payments
    ADD CONSTRAINT fk_payments_ticket FOREIGN KEY (ticket_id) REFERENCES tickets(id)  ON DELETE RESTRICT;

ALTER TABLE validations
    ADD CONSTRAINT fk_validations_ticket FOREIGN KEY (ticket_id) REFERENCES tickets(id) ON DELETE RESTRICT;

ALTER TABLE trips
    ADD CONSTRAINT fk_trips_vehicle   FOREIGN KEY (vehicle_id) REFERENCES vehicles(id) ON DELETE RESTRICT;

ALTER TABLE vehicles
    ADD CONSTRAINT fk_vehicles_operator FOREIGN KEY (operator_id) REFERENCES operators(id) ON DELETE RESTRICT;

-- Status constraints to prevent illegal values at the storage boundary
ALTER TABLE tickets     ADD CONSTRAINT chk_tickets_status
    CHECK (status IN ('Paid', 'Validated', 'Cancelled', 'Refunded'));

ALTER TABLE payments    ADD CONSTRAINT chk_payments_status
    CHECK (status IN ('Captured', 'Failed', 'Refunded', 'Pending'));

ALTER TABLE trips       ADD CONSTRAINT chk_trips_status
    CHECK (status IN ('Scheduled', 'Cancelled', 'Completed'));

ALTER TABLE validations ADD CONSTRAINT chk_validations_result
    CHECK (result IN ('Accepted', 'Rejected'));
```

---

## Issue 10 [LOW] - In-Memory Event Queue Loses Events on Restart

**Before** (`BackgroundQueueEventPublisher.cs`): plain .NET channel - no persistence.

```csharp
private readonly Channel<object> _channel = Channel.CreateUnbounded<object>();
// All unconsumed events are lost if the process restarts
```

**After**: PostgreSQL outbox table written in the same transaction as the domain event; a separate poller dispatches and deletes rows.

```sql
-- 008_outbox.sql
CREATE TABLE outbox (
    id          bigserial    PRIMARY KEY,
    event_type  text         NOT NULL,
    payload     jsonb        NOT NULL,
    created_utc timestamptz  NOT NULL DEFAULT now(),
    sent_utc    timestamptz
);
CREATE INDEX idx_outbox_unsent ON outbox (id) WHERE sent_utc IS NULL;
```

Write the event inside the same transaction as the domain write:

```csharp
// Inside the timetable replacement transaction:
await connection.ExecuteAsync("""
    INSERT INTO outbox (event_type, payload)
    VALUES (@type, @payload::jsonb)
    """,
    new { type = nameof(TimetableChangedEvent), payload = JsonSerializer.Serialize(evt) },
    transaction);
```

Poller dispatches and marks rows as sent - events survive any process restart:

```csharp
// OutboxPollerWorker - runs on a timer, e.g. every 5 seconds
var rows = await connection.QueryAsync(
    "SELECT * FROM outbox WHERE sent_utc IS NULL ORDER BY id LIMIT 100");

foreach (var row in rows)
{
    await broker.PublishAsync(row.event_type, row.payload);
    await connection.ExecuteAsync(
        "UPDATE outbox SET sent_utc = now() WHERE id = @id", new { row.id });
}
```
