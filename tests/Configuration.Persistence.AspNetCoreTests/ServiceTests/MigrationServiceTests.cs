namespace Kritikos.Configuration.PersistenceTests.ServiceTests
{
  using System;
  using System.Linq;
  using System.Threading.Tasks;

  using Kritikos.Configuration.Persistence.AspNetCore.Services;
  using Kritikos.Samples.CityCensus;

  using Microsoft.AspNetCore.Builder;
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.AspNetCore.Http;
  using Microsoft.AspNetCore.TestHost;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.DependencyInjection;

  using Xunit;

  public class MigrationServiceTests : IClassFixture<SampleDbContextFixture>
  {
    private readonly SampleDbContextFixture fixture;

    public MigrationServiceTests(SampleDbContextFixture fixture) => this.fixture = fixture;

    [Fact]
    public async Task DbContext_has_Migrations()
    {
      var ctx = await fixture.GetContext("transient");
      var migrations = (await ctx.Database.GetPendingMigrationsAsync()).ToList();

      Assert.NotEmpty(migrations);
    }

    [Fact]
    [Obsolete("Recommending extension method instead.")]
    public async Task Ensure_Context_IsMigrated_by_Service()
    {
      var ctx = await fixture.GetContext("migrate_service");
      var migrations = (await ctx.Database.GetPendingMigrationsAsync()).ToList();
      Assert.NotEmpty(migrations);

      var builder = new WebHostBuilder()
        .ConfigureServices(sp =>
        {
          sp.AddScoped(_ => ctx);
          sp.AddHostedService<MigrationService<CityCensusTrailDbContext>>();
        })
        .Configure(app => app.Run(async context => await context.Response.WriteAsync("Hello world!")));


      using (var server = new TestServer(builder))
      using (var client = server.CreateClient())
      {
        var response = await client.GetAsync("/");
        Assert.True(response.IsSuccessStatusCode);
      }

      await using var ctx2 = await fixture.GetContext("migrate_service");
      migrations = (await ctx2.Database.GetAppliedMigrationsAsync()).ToList();
      Assert.NotEmpty(migrations);

      migrations = (await ctx2.Database.GetPendingMigrationsAsync()).ToList();
      Assert.Empty(migrations);
    }
  }
}
