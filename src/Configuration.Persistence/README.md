# Configuration.Persistence

Convention-based model configuration for Entity Framework Core, turning the behavioural contracts into actual mapping.

## Getting Started

```shell
dotnet add package Kritikos.Configuration.Persistence
```

The package brings in [`Kritikos.Configuration.Persistence.Contracts`](../Configuration.Persistence.Contracts/README.md) and `Microsoft.EntityFrameworkCore.Relational`. It stays provider-agnostic, so pick your own provider package alongside it.

```csharp
protected override void OnModelCreating(ModelBuilder builder)
{
  base.OnModelCreating(builder);

  builder.ApplyConcurrencyTokens();
  builder.ApplySoftDeletableFilters();
}
```

## Capabilities

| Extension | Extends | Effect |
| --- | --- | --- |
| `EntitiesImplementing<T>` | `ModelBuilder` | Runs a configuration action against every entity assignable to interface `T` |
| `EntitiesOfType<T>` | `ModelBuilder` | Configures base class `T` once through a strongly typed `EntityTypeBuilder<T>`, leaving derived types to inherit it |
| `ApplyConcurrencyTokens` | `ModelBuilder` | Registers the row version of every `IConcurrent` entity as a concurrency token |
| `ApplySoftDeletableFilters` | `ModelBuilder` | Defaults `IsDeleted` to `false` and adds a global query filter excluding deleted rows, declared on each inheritance root |
| `ManyToManyWithJoinEntity` | `EntityTypeBuilder<T>` | Configures a join entity with a composite key built from both sides |
| `ManyToManyWithSkipNavigation` | `EntityTypeBuilder<T>` | Configures a skip navigation backed by an explicit join entity |
| `EnableCommonOptions` | `DbContextOptionsBuilder` | Applies the shared diagnostics and cascade-delete policy |

The package also contributes the concurrency contracts and the entity that backs audit trails.

| Type | Purpose |
| --- | --- |
| `IConcurrent` | Marker for entities under optimistic concurrency |
| `IPostgreSqlConcurrent` | Exposes the PostgreSQL `xmin` system column as a `uint` |
| `ISqlServerConcurrent` | Exposes the SQL Server `rowversion` column as a `byte[]` |
| `IAuditTrailDbContext<TAudit>` | Exposes the `DbSet<TAudit>` that audit interceptors write into |
| `AuditRecord` | A single audit entry: table, key, `EntityState`, and old and new values |

## Usage Examples

`EntitiesImplementing<T>` is the escape hatch for anything the built-in conventions do not cover, including provider-specific configuration.

```csharp
builder.EntitiesImplementing<IPostgreSqlConcurrent>(entity =>
  entity.Property<uint>(nameof(IPostgreSqlConcurrent.RowVersion)).UseXminAsConcurrencyToken());
```

`ManyToManyWithSkipNavigation` keeps a many-to-many navigable from both sides while retaining an explicit join entity you can extend with payload columns.

```csharp
builder.Entity<Person>()
  .ManyToManyWithSkipNavigation<PersonCorporation, Person, Corporation>(
    person => person.Corporations,
    corporation => corporation.Employees);
```

`ManyToManyWithJoinEntity` targets the join entity instead, deriving a composite key from both foreign keys so no surrogate key is needed.

```csharp
builder.Entity<PersonCorporation>()
  .ManyToManyWithJoinEntity(
    join => join.Person,
    join => join.Corporation);
```

Audit trails need a context exposing the record set; `AuditRecord.OnModelCreating` configures the key, persists `Modification` as text rather than as an opaque integer, and indexes the two ways a trail is read: `(Table, Key)` for the history of one row, and `CreatedAt` for everything that happened within a period.

```csharp
public class AppDbContext : DbContext, IAuditTrailDbContext<AuditRecord>
{
  public DbSet<AuditRecord> AuditRecords => Set<AuditRecord>();

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);
    AuditRecord.OnModelCreating(builder);
  }
}
```

## Caveats

> [!WARNING]
> `ApplySoftDeletableFilters` installs a global query filter. Rows hidden by it are invisible to `Include` and to foreign-key fixup, which surfaces as a required navigation resolving to `null`. Use `IgnoreQueryFilters()` where deleted rows are genuinely wanted.

> [!NOTE]
> Entity Framework Core only accepts a query filter on the root of an inheritance hierarchy, so `ApplySoftDeletableFilters` declares one filter per root. Derived soft-deletable types are covered by their root's filter rather than one of their own.

> [!IMPORTANT]
> `EnableCommonOptions` escalates `CascadeDelete` and `CascadeDeleteOrphan` to thrown exceptions. This is deliberate — cascading deletes should be declared, not inherited from a convention — but it will break a model that relies on the default behaviour.

> [!CAUTION]
> `EnableCommonOptions` enables sensitive data logging when `isDevelopment` is `true`, which writes parameter values into the log. Never pass `true` in an environment whose logs you do not fully control.

> [!NOTE]
> `ManyToManyWithJoinEntity` assumes both sides have a single-property primary key. Configure composite-keyed relationships manually.

> [!NOTE]
> `IPostgreSqlShadowConcurrent` is obsolete; use `IPostgreSqlConcurrent`, which exposes the concurrency token as a real property rather than a shadow one.
