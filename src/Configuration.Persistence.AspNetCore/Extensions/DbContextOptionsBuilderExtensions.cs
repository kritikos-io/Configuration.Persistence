namespace Kritikos.Configuration.Persistence.AspNetCore.Extensions;

using System;

using Kritikos.Configuration.Persistence.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

/// <summary>
/// Extension methods applying the repository's common <see cref="DbContextOptionsBuilder"/> configuration
/// based on the hosting environment.
/// </summary>
public static class DbContextOptionsBuilderExtensions
{
  /// <summary>
  /// Enables logging and sensitive data exposure if configuring on a development environment, suppresses those warnings from logs and prevents cascading deletions.
  /// </summary>
  /// <param name="builder">The builder to operate on.</param>
  /// <param name="environment">The environment to enable development specific options.</param>
  /// <returns>The same builder instance so that multiple calls can be chained.</returns>
  /// <exception cref="ArgumentNullException"><paramref name="builder"/> or <paramref name="environment"/> is null.</exception>
  public static DbContextOptionsBuilder EnableCommonOptions(
    this DbContextOptionsBuilder builder,
    IHostEnvironment environment)
  {
    ArgumentNullException.ThrowIfNull(builder);
    ArgumentNullException.ThrowIfNull(environment);

    return builder.EnableCommonOptions(environment.IsDevelopment());
  }

  /// <summary>
  /// Enables logging and sensitive data exposure if configuring on a development environment, suppresses those warnings from logs and prevents cascading deletions.
  /// </summary>
  /// <typeparam name="TContext">Type of the <seealso cref="DbContext"/>.</typeparam>
  /// <param name="builder">The builder to operate on.</param>
  /// <param name="environment">The environment to enable development specific options.</param>
  /// <returns>The same builder instance so that multiple calls can be chained.</returns>
  /// <exception cref="ArgumentNullException"><paramref name="builder"/> or <paramref name="environment"/> is null.</exception>
  public static DbContextOptionsBuilder<TContext> EnableCommonOptions<TContext>(
    this DbContextOptionsBuilder<TContext> builder,
    IHostEnvironment environment)
    where TContext : DbContext
  {
    ArgumentNullException.ThrowIfNull(builder);
    ArgumentNullException.ThrowIfNull(environment);

    return builder.EnableCommonOptions(environment.IsDevelopment());
  }
}
