namespace Kritikos.Configuration.PersistenceTests.ServiceTests
{
  using System.Linq;
  using System.Threading.Tasks;

  using FluentAssertions;

  using Kritikos.Configuration.Persistence.Services;
  using Kritikos.Configuration.TestData;

  using Microsoft.AspNetCore.Builder;
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.AspNetCore.Http;
  using Microsoft.AspNetCore.TestHost;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.DependencyInjection;

  using Xunit;

  public class MigrationServiceTests : IClassFixture<DbContextFixture>
  {
    private readonly DbContextFixture fixture;

    public MigrationServiceTests(DbContextFixture fixture) => this.fixture = fixture;

    [Fact]
    public async Task DbContext_has_Migrations()
    {
      var ctx = await fixture.GetContext("transient");
      var migrations = (await ctx.Database.GetPendingMigrationsAsync()).ToList();

      migrations.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Ensure_Context_IsMigrated()
    {
      var ctx = await fixture.GetContext("migrate");

      var builder = new WebHostBuilder()
        .ConfigureServices(sp =>
        {
          sp.AddScoped(_ => ctx);
          sp.AddHostedService<MigrationService<MigratedDbContext>>();
        })
        .Configure(app => app.Run(async context => await context.Response.WriteAsync("Hello world!")));

      var server = new TestServer(builder);
      var client = server.CreateClient();

      var response = await client.GetAsync("/");
      response.IsSuccessStatusCode.Should().BeTrue();

      client.Dispose();
      server.Dispose();

      await using var ctx2 = await fixture.GetContext("migrate");
      var migrations = (await ctx2.Database.GetAppliedMigrationsAsync()).ToList();
      migrations.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Ensure_all_migrations_applied()
    {
      var ctx = await fixture.GetContext("migrate");
      var migrations = (await ctx.Database.GetPendingMigrationsAsync()).ToList();

      migrations.Should().BeEmpty();
    }
  }
}
