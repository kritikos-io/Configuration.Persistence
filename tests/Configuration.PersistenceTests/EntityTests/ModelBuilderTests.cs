namespace Kritikos.Configuration.PersistenceTests.EntityTests;

using System;
using System.Threading.Tasks;

using Kritikos.Samples.CityCensus;

using Microsoft.EntityFrameworkCore;

using Xunit;

public class ModelBuilderTests(SampleDbContextFixture fixture)
  : IClassFixture<SampleDbContextFixture>
{
  [Fact]
  public async Task EntitiesOfType_by_Interface()
  {
    await using var ctx = await fixture.GetContextAsync("ofType_interface");
    await ctx.Database.MigrateAsync();

    var counties = CityDataFaker.Counties.Generate(20);
    Assert.All(counties, c => Assert.True(c.Order == Guid.Empty));

    ctx.Counties.AddRange(counties);
    await ctx.SaveChangesAsync();

    Assert.All(counties, c => Assert.False(c.Order == Guid.Empty));
  }

  [Fact]
  public async Task EntitiesOfType_by_BaseClass()
  {
    await using var ctx = await fixture.GetContextAsync("ofType_base");
    await ctx.Database.MigrateAsync();

    var counties = CityDataFaker.Counties.Generate(20);
    Assert.All(counties, c => Assert.True(c.Order == Guid.Empty));

    ctx.Counties.AddRange(counties);
    await ctx.SaveChangesAsync();

    Assert.All(counties, c => Assert.False(c.Order == Guid.Empty));
  }
}
