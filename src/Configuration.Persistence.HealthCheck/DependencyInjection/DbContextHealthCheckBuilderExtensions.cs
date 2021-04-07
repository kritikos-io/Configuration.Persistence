namespace Kritikos.Configuration.Persistence.HealthCheck.DependencyInjection
{
  using System;
  using System.Collections.Generic;

  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Diagnostics.HealthChecks;
  using Microsoft.Extensions.Logging;

  public static class DbContextHealthCheckBuilderExtensions
  {
    private const string Name = "DbContext";

    public static IHealthChecksBuilder AddDbContext<TDbContext>(
      this IHealthChecksBuilder builder,
      string? name = default,
      HealthStatus? failureStatus = default,
      IEnumerable<string>? tags = default,
      TimeSpan? timeout = default)
      where TDbContext : DbContext
      => builder.Add(new HealthCheckRegistration(
        name ?? Name,
        sp => new DbContextHealthCheck<TDbContext>(
          sp.GetRequiredService<TDbContext>(),
          sp.GetRequiredService<ILogger<DbContextHealthCheck<TDbContext>>>()),
        failureStatus,
        tags,
        timeout));
  }
}
