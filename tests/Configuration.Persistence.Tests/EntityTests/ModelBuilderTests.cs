namespace Kritikos.Configuration.Persistence.Tests.EntityTests;

using System;
using System.Threading.Tasks;

using Kritikos.Configuration.Persistence.TestKit;
using Kritikos.Samples.CityCensus;

using Microsoft.EntityFrameworkCore;

[ClassDataSource<SampleDbContextFixture>(Shared = SharedType.PerClass)]
public class ModelBuilderTests(SampleDbContextFixture fixture)
{
  private readonly SampleDbContextFixture fixture = fixture;

  [Test]
  public async Task SaveChangesAsync_ConventionConfiguredValueGenerator_PopulatesOrder(
    CancellationToken cancellationToken)
  {
    await using var ctx = await fixture.GetContextAsync("convention_value_generator");
    await ctx.Database.MigrateAsync(cancellationToken);

    var counties = CityDataFaker.Counties.Generate(20);
    foreach (var county in counties)
    {
      await Assert.That(county.Order).IsEqualTo(Guid.Empty);
    }

    ctx.Counties.AddRange(counties);
    await ctx.SaveChangesAsync(cancellationToken);

    foreach (var county in counties)
    {
      await Assert.That(county.Order).IsNotEqualTo(Guid.Empty);
    }
  }
}
