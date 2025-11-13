namespace Kritikos.Configuration.Persistence.AspNetCore;

using Microsoft.Extensions.Logging;

internal static partial class LoggerMessages
{
  [LoggerMessage(LogLevel.Warning, "Applying pending migrations to {DbContext}: {Migrations}")]
  public static partial void LogApplyingMigrations(this ILogger logger, string dbContext, string[] migrations);

  [LoggerMessage(LogLevel.Warning, "Migrations for {DbContext} have been applied succesfully")]
  public static partial void LogMigrationsAppliedSuccessfully(this ILogger logger, string dbContext);
}
