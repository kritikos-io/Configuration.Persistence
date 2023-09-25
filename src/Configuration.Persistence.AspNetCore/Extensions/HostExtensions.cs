namespace Kritikos.Configuration.Persistence.AspNetCore.Extensions;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public static class HostExtensions
{
  /// <summary>
  /// Applies pending migrations on <typeparamref name="TDbContext"/>.
  /// </summary>
  /// <typeparam name="TDbContext">The <see cref="DbContext"/> to migrate.</typeparam>
  /// <param name="host">The <see cref="IHost"/> to operate on.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
  /// <returns>A <see cref="Task"/> representing the asynchronous migration operation.</returns>
  public static async Task MigrateAsync<TDbContext>(this IHost host, CancellationToken cancellationToken = default)
    where TDbContext : DbContext
  {
    ArgumentNullException.ThrowIfNull(host);

    using var scope = host.Services.CreateScope();
    var contextName = typeof(TDbContext).Name;

    var ctx = scope.ServiceProvider.GetService<TDbContext>()
              ?? throw new InvalidOperationException($"Could not resolve an instance of {contextName}");

    var logger = scope.ServiceProvider.GetRequiredService<ILogger<TDbContext>>();

    var migrations = (await ctx.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();
    if (migrations.Count == 0)
    {
      return;
    }

    logger.LogInformation("Applying migrations to {DbContext}: {Migrations}", contextName, migrations);
    await ctx.Database.MigrateAsync(cancellationToken);
    logger.LogInformation("Migrations for {DbContext} completed succesfully", contextName);
  }
}
