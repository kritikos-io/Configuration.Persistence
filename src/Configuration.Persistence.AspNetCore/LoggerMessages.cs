namespace Kritikos.Configuration.Persistence.AspNetCore;

using Microsoft.Extensions.Logging;

/// <summary>
/// Source-generated log messages emitted while applying migrations.
/// </summary>
internal static partial class LoggerMessages
{
  /// <summary>
  /// Logs the set of migrations about to be applied.
  /// </summary>
  /// <param name="logger">The logger to write to.</param>
  /// <param name="dbContext">Name of the context being migrated.</param>
  /// <param name="migrations">The pending migrations.</param>
  [LoggerMessage(LogLevel.Warning, "Applying pending migrations to {DbContext}: {Migrations}")]
  public static partial void LogApplyingMigrations(this ILogger logger, string dbContext, string[] migrations);

  /// <summary>
  /// Logs that every pending migration was applied without error.
  /// </summary>
  /// <param name="logger">The logger to write to.</param>
  /// <param name="dbContext">Name of the context that was migrated.</param>
  [LoggerMessage(LogLevel.Warning, "Migrations for {DbContext} have been applied successfully")]
  public static partial void LogMigrationsAppliedSuccessfully(this ILogger logger, string dbContext);
}
