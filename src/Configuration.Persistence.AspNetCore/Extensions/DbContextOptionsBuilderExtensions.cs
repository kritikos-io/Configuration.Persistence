namespace Kritikos.Configuration.Persistence.AspNetCore.Extensions
{
  using Kritikos.Configuration.Persistence.Extensions;

  using Microsoft.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore.Diagnostics;
  using Microsoft.Extensions.Hosting;

  public static class DbContextOptionsBuilderExtensions
  {
    /// <summary>
    /// Enables logging and sensitive data exposure if configuring on a development environment, supresses those warnings from logs and prevents cascading deletions.
    /// </summary>
    /// <param name="builder">The builder to operate on.</param>
    /// <param name="environment">The environment to enable development specific options.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static DbContextOptionsBuilder EnableCommonOptions(
      this DbContextOptionsBuilder builder,
      IHostEnvironment environment)
      => builder.EnableCommonOptions(environment.IsDevelopment());

    /// <summary>
    /// Enables logging and sensitive data exposure if configuring on a development environment, supresses those warnings from logs and prevents cascading deletions.
    /// </summary>
    /// <typeparam name="TContext">Type of the <seealso cref="DbContext"/>.</typeparam>
    /// <param name="builder">The builder to operate on.</param>
    /// <param name="environment">The environment to enable development specific options.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static DbContextOptionsBuilder<TContext> EnableCommonOptions<TContext>(
      this DbContextOptionsBuilder<TContext> builder,
      IHostEnvironment environment)
      where TContext : DbContext
      => builder.EnableCommonOptions(environment.IsDevelopment());
  }
}
