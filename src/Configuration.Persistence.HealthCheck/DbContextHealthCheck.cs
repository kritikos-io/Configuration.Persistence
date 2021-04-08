namespace Kritikos.Configuration.Persistence.HealthCheck
{
  using System;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;

  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.Diagnostics.HealthChecks;
  using Microsoft.Extensions.Logging;

  public class DbContextHealthCheck<TDbContext> : IHealthCheck
    where TDbContext : DbContext
  {
    private static readonly string DbName = typeof(TDbContext).Name;

    private readonly TDbContext dbContext;
    private readonly ILogger logger;

    public DbContextHealthCheck(TDbContext context, ILogger<DbContextHealthCheck<TDbContext>> logger)
    {
      dbContext = context;
      this.logger = logger;
    }

    #region Implementation of IHealthCheck

    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(
      HealthCheckContext context,
      CancellationToken cancellationToken = default)
    {
      try
      {
        var database = dbContext.Database;

        logger.LogDebug(DbContextHealthLogTemplates.AttemptingToConnect, DbName);
        var canConnect = await database.CanConnectAsync(cancellationToken);

        if (!canConnect)
        {
          logger.LogDebug(DbContextHealthLogTemplates.ConnectionFailed, DbName);
          return new HealthCheckResult(
            context.Registration.FailureStatus,
            description: $"Database {DbName} not responding");
        }

        logger.LogInformation(DbContextHealthLogTemplates.DatabaseConnected, DbName);

        logger.LogDebug(DbContextHealthLogTemplates.CheckingForMigrations, DbName);
        var pendingMigrations = (await database.GetPendingMigrationsAsync(cancellationToken)).ToList();

        if (pendingMigrations.Any())
        {
          var data = pendingMigrations.ToDictionary<string, string, object>(
            migration => migration,
            _ => string.Empty);

          logger.LogWarning(
            DbContextHealthLogTemplates.PendingMigrations,
            DbName,
            pendingMigrations.Count,
            pendingMigrations);
          return HealthCheckResult.Degraded($"{DbName} is operating but is missing migrations", data: data);
        }

        logger.LogInformation(DbContextHealthLogTemplates.DatabaseMigrated, DbName);

        return HealthCheckResult.Healthy();
      }
      catch (Exception ex)
      {
        logger.LogCritical(ex, DbContextHealthLogTemplates.UnknownError, DbName);
        return new HealthCheckResult(
          HealthStatus.Unhealthy,
          $"Unknown error while connecting to database {DbName}",
          ex);
      }
    }

    #endregion
  }
}
