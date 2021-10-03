namespace Kritikos.Configuration.Persistence.Extensions
{
  using System.Diagnostics.CodeAnalysis;

  using Microsoft.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore.Diagnostics;

  [ExcludeFromCodeCoverage]
  public static class DbContextOptionsBuilderExtensions
  {
    /// <summary>
    /// Enables logging and sensitive data exposure if configuring on a development environment, supresses those warnings from logs and prevents cascading deletions.
    /// </summary>
    /// <param name="builder">The builder to operate on.</param>
    /// <param name="isDevelopment">If used on a development environment, suppresses warnings for sensitive data.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static DbContextOptionsBuilder EnableCommonOptions(
      this DbContextOptionsBuilder builder,
      bool isDevelopment)
      => builder
        .EnableDetailedErrors(isDevelopment)
        .EnableSensitiveDataLogging(isDevelopment)
        .ConfigureWarnings(warn => warn
          .Ignore(CoreEventId.SensitiveDataLoggingEnabledWarning)
          .Log(RelationalEventId.QueryPossibleUnintendedUseOfEqualsWarning)
          .Throw(CoreEventId.CascadeDelete, CoreEventId.CascadeDeleteOrphan));

    /// <summary>
    /// Enables logging and sensitive data exposure if configuring on a development environment, supresses those warnings from logs and prevents cascading deletions.
    /// </summary>
    /// <typeparam name="TContext">Type of the <seealso cref="DbContext"/>.</typeparam>
    /// <param name="builder">The builder to operate on.</param>
    /// <param name="isDevelopment">If used on a development environment, suppresses warnings for sensitive data.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static DbContextOptionsBuilder<TContext> EnableCommonOptions<TContext>(
      this DbContextOptionsBuilder<TContext> builder,
      bool isDevelopment)
      where TContext : DbContext
      => builder
        .EnableDetailedErrors(isDevelopment)
        .EnableSensitiveDataLogging(isDevelopment)
        .ConfigureWarnings(warn => warn
          .Ignore(CoreEventId.SensitiveDataLoggingEnabledWarning)
          .Log(RelationalEventId.QueryPossibleUnintendedUseOfEqualsWarning)
          .Throw(CoreEventId.CascadeDelete, CoreEventId.CascadeDeleteOrphan));
  }
}
