# Configuration.Persistence.AspNetCore

Host and options wiring that connects the persistence extensions to an ASP.NET Core application's environment and lifetime.

## Getting Started

```shell
dotnet add package Kritikos.Configuration.Persistence.AspNetCore
```

The package adds environment-aware overloads on top of [`Kritikos.Configuration.Persistence`](../Configuration.Persistence/README.md), so the hosting environment decides which development-only diagnostics are enabled.

```csharp
builder.Services.AddDbContext<AppDbContext>(options => options
  .UseNpgsql(connectionString)
  .EnableCommonOptions(builder.Environment));

var app = builder.Build();
await app.MigrateAsync<AppDbContext>();
await app.RunAsync();
```

## Capabilities

| Extension | Extends | Effect |
| --- | --- | --- |
| `EnableCommonOptions(IHostEnvironment)` | `DbContextOptionsBuilder` | Applies the shared diagnostics policy, enabling detailed errors and sensitive data logging only in development |
| `MigrateAsync<TDbContext>` | `IHost` | Applies pending migrations before the host starts serving |

`MigrateAsync` resolves the context from a new service scope, checks for pending migrations, and returns immediately when there are none. When there are, it logs the migration names, applies them, and logs completion.

## Usage Examples

Migrating before `RunAsync` guarantees the schema is current before the first request is accepted.

```csharp
var app = builder.Build();

await app.MigrateAsync<AppDbContext>();

app.MapControllers();
await app.RunAsync();
```

Both overloads of `EnableCommonOptions` are available, including the generic form for a typed options builder.

```csharp
builder.Services.AddDbContext<AppDbContext>((provider, options) => options
  .UseNpgsql(connectionString)
  .EnableCommonOptions(provider.GetRequiredService<IHostEnvironment>()));
```

## Caveats

> [!WARNING]
> Migrating at startup serialises poorly. Every replica of a scaled-out deployment will attempt the same migration concurrently, and the losers fail on an already-applied migration. Run migrations as a separate step of your deployment when more than one instance can start at once.

> [!IMPORTANT]
> `MigrateAsync` throws `InvalidOperationException` when `TDbContext` is not registered. It requires a relational provider, since the in-memory provider does not support migrations.

> [!CAUTION]
> `EnableCommonOptions` enables sensitive data logging in the development environment, which writes parameter values into the log. Confirm that nothing running with `ASPNETCORE_ENVIRONMENT=Development` ships its logs somewhere you would not want query parameters to appear.
