namespace Kritikos.Configuration.Persistence.AspNetCoreTests.ExtensionTests;

using System.Linq;
using System.Threading.Tasks;

using Kritikos.Configuration.Persistence.AspNetCore.Extensions;
using Kritikos.Configuration.PersistenceTests;
using Kritikos.Samples.CityCensus;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

[ClassDataSource<SampleDbContextFixture>(Shared = SharedType.PerClass)]
public class MigrationExtensionTests(SampleDbContextFixture fixture)
{
  [Test]
  public async Task Ensure_HostExtension_Migrates(CancellationToken cancellationToken)
  {
    await using var ctx = await fixture.GetContextAsync("migrate_extension");
    var migrations = (await ctx.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();
    await Assert.That(migrations).IsNotEmpty();

    var builder = Host.CreateDefaultBuilder()
      .ConfigureWebHostDefaults(webBuilder =>
      {
        webBuilder.ConfigureServices(sp =>
        {
          sp.AddScoped(_ => ctx);
        });

        webBuilder.Configure(app => app.Run(async context => await context.Response.WriteAsync("Hello world!")));
      });

    using (var host = builder.Build())
    {
      await host.MigrateAsync<CityCensusTrailDbContext>(cancellationToken);
    }

    await using var ctx2 = await fixture.GetContextAsync("migrate_extension");

    migrations = [.. await ctx2.Database.GetPendingMigrationsAsync(cancellationToken)];
    await Assert.That(migrations).IsEmpty();

    migrations = [.. await ctx2.Database.GetAppliedMigrationsAsync(cancellationToken)];
    await Assert.That(migrations).IsNotEmpty();
  }
}
