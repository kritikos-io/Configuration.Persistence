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
  public async Task EntitiesImplementing_EntityConfiguredByInterface_GeneratesOrderOnSave(CancellationToken cancellationToken)
  {
    await using var ctx = await fixture.GetContextAsync("ofType_interface");
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

  [Test]
  public async Task EntitiesOfType_EntityConfiguredByBaseClass_GeneratesOrderOnSave(CancellationToken cancellationToken)
  {
    await using var ctx = await fixture.GetContextAsync("ofType_base");
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
