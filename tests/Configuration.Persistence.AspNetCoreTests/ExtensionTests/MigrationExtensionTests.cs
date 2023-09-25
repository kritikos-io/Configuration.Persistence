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

using Xunit;

public class MigrationExtensionTests(SampleDbContextFixture fixture)
  : IClassFixture<SampleDbContextFixture>
{
  [Fact]
  public async Task Ensure_HostExtension_Migrates()
  {
    await using var ctx = await fixture.GetContext("migrate_extension");
    var migrations = (await ctx.Database.GetPendingMigrationsAsync()).ToList();
    Assert.NotEmpty(migrations);

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
      await host.MigrateAsync<CityCensusTrailDbContext>();
    }

    await using var ctx2 = await fixture.GetContext("migrate_extension");

    migrations = (await ctx2.Database.GetPendingMigrationsAsync()).ToList();
    Assert.Empty(migrations);

    migrations = (await ctx2.Database.GetAppliedMigrationsAsync()).ToList();
    Assert.NotEmpty(migrations);
  }
}
