namespace Kritikos.Configuration.Persistence.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

/// <summary>
/// Extension methods applying the repository's common <see cref="DbContextOptionsBuilder"/> configuration.
/// </summary>
public static class DbContextOptionsBuilderExtensions
{
  /// <summary>
  /// Enables logging and sensitive data exposure if configuring on a development environment, suppresses those warnings from logs and prevents cascading deletions.
  /// </summary>
  /// <param name="builder">The builder to operate on.</param>
  /// <param name="isDevelopment">If used on a development environment, suppresses warnings for sensitive data.</param>
  /// <returns>The same builder instance so that multiple calls can be chained.</returns>
  /// <exception cref="ArgumentNullException"><paramref name="builder"/> is null.</exception>
  public static DbContextOptionsBuilder EnableCommonOptions(
    this DbContextOptionsBuilder builder,
    bool isDevelopment)
  {
    ArgumentNullException.ThrowIfNull(builder);

    return ApplyCommonOptions(builder, isDevelopment);
  }

  /// <summary>
  /// Enables logging and sensitive data exposure if configuring on a development environment, suppresses those warnings from logs and prevents cascading deletions.
  /// </summary>
  /// <typeparam name="TContext">Type of the <seealso cref="DbContext"/>.</typeparam>
  /// <param name="builder">The builder to operate on.</param>
  /// <param name="isDevelopment">If used on a development environment, suppresses warnings for sensitive data.</param>
  /// <returns>The same builder instance so that multiple calls can be chained.</returns>
  /// <exception cref="ArgumentNullException"><paramref name="builder"/> is null.</exception>
  public static DbContextOptionsBuilder<TContext> EnableCommonOptions<TContext>(
    this DbContextOptionsBuilder<TContext> builder,
    bool isDevelopment)
    where TContext : DbContext
  {
    ArgumentNullException.ThrowIfNull(builder);

    ApplyCommonOptions(builder, isDevelopment);

    return builder;
  }

  private static DbContextOptionsBuilder ApplyCommonOptions(DbContextOptionsBuilder builder, bool isDevelopment)
    => builder
      .EnableDetailedErrors(isDevelopment)
      .EnableSensitiveDataLogging(isDevelopment)
      .ConfigureWarnings(warn => warn
        .Ignore(CoreEventId.SensitiveDataLoggingEnabledWarning)
        .Log(RelationalEventId.QueryPossibleUnintendedUseOfEqualsWarning)
        .Throw(CoreEventId.CascadeDelete, CoreEventId.CascadeDeleteOrphan));
}
