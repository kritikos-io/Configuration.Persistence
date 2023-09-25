namespace Kritikos.Configuration.Persistence.InterceptorTests;

using System;
using System.Threading.Tasks;

using Kritikos.Configuration.Persistence.Interceptors.SaveChanges;
using Kritikos.Configuration.PersistenceTests;
using Kritikos.Samples.CityCensus;

using Microsoft.EntityFrameworkCore;

using Xunit;

public class TimeStampedInterceptorTests(SampleDbContextFixture fixture)
  : IClassFixture<SampleDbContextFixture>
{
  [Fact]
  public async Task CreatedAt_Is_Populated()
  {
    await using var ctx = await fixture.GetContext("createdAt", new TimestampSaveChangesInterceptor());
    await ctx.Database.MigrateAsync();
    var counties = CityDataFaker.Counties.Generate(10);
    ctx.AddRange(counties);

    var then = DateTimeOffset.Now;
    await ctx.SaveChangesAsync();
    var now = DateTimeOffset.Now;

    Assert.All(counties, c =>
    {
      Assert.True(c.CreatedAt >= then);
      Assert.Equal(c.CreatedAt, c.UpdatedAt);
      Assert.True(c.CreatedAt <= now);
    });
  }

  [Fact]
  public async Task UpdatedAt_Is_Altered()
  {
    await using var ctx = await fixture.GetContext("createdAt", new TimestampSaveChangesInterceptor());
    await ctx.Database.MigrateAsync();
    var counties = CityDataFaker.Counties.Generate(10);
    ctx.AddRange(counties);

    await ctx.SaveChangesAsync();
    var then = DateTimeOffset.Now;

    foreach (var county in counties)
    {
      county.Name = "REDUCTED";
    }

    await ctx.SaveChangesAsync();
    var now = DateTimeOffset.Now;

    Assert.All(counties, c =>
    {
      Assert.True(c.UpdatedAt >= then);
      Assert.True(c.CreatedAt < c.UpdatedAt);
      Assert.True(c.UpdatedAt <= now);
    });
  }
}
