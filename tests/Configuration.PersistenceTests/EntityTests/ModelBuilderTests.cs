namespace Kritikos.Configuration.PersistenceTests.EntityTests
{
  using System;

  using Kritikos.Samples.CityCensus.Provider;

  using Microsoft.EntityFrameworkCore;

  using Xunit;

  public class ModelBuilderTests : IClassFixture<SampleDbContextFixture>
  {
    private readonly SampleDbContextFixture fixture;

    public ModelBuilderTests(SampleDbContextFixture fixture)
    {
      this.fixture = fixture;
    }

    [Fact]
    public async Task EntitiesOfType_by_Interface()
    {
      await using var ctx = await fixture.GetContext("ofType_interface");
      await ctx.Database.MigrateAsync();

      var counties = CountyProvider.Provider.Generate(20);
      Assert.All(counties, c => Assert.True(c.Order == Guid.Empty));

      ctx.Counties.AddRange(counties);
      await ctx.SaveChangesAsync();

      Assert.All(counties, c => Assert.False(c.Order == Guid.Empty));
    }

    [Fact]
    public async Task EntitiesOfType_by_BaseClass()
    {
      await using var ctx = await fixture.GetContext("ofType_base");
      await ctx.Database.MigrateAsync();

      var counties = CountyProvider.Provider.Generate(20);
      Assert.All(counties, c => Assert.True(c.Order == Guid.Empty));

      ctx.Counties.AddRange(counties);
      await ctx.SaveChangesAsync();

      Assert.All(counties, c => Assert.False(c.Order == Guid.Empty));
    }
  }
}
