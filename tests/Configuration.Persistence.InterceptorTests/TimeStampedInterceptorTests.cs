namespace Kritikos.Configuration.Persistence.InterceptorTests;

using System;
using System.Threading.Tasks;

using Kritikos.Configuration.Persistence.Interceptors.SaveChanges;
using Kritikos.Configuration.PersistenceTests;
using Kritikos.Samples.CityCensus;

using Microsoft.EntityFrameworkCore;

// Both tests share the "createdAt" in-memory database, which SQLite keys globally by name,
// so they must not run concurrently.
[ClassDataSource<SampleDbContextFixture>(Shared = SharedType.PerClass)]
[NotInParallel]
public class TimeStampedInterceptorTests(SampleDbContextFixture fixture)
{
  [Test]
  public async Task CreatedAt_Is_Populated(CancellationToken cancellationToken)
  {
    await using var ctx = await fixture.GetContextAsync("createdAt", new TimestampSaveChangesInterceptor());
    await ctx.Database.MigrateAsync(cancellationToken);
    var counties = CityDataFaker.Counties.Generate(10);
    ctx.AddRange(counties);

    var then = DateTimeOffset.Now;
    await ctx.SaveChangesAsync(cancellationToken);
    var now = DateTimeOffset.Now;

    foreach (var county in counties)
    {
      await Assert.That(county.CreatedAt >= then).IsTrue();
      await Assert.That(county.UpdatedAt).IsEqualTo(county.CreatedAt);
      await Assert.That(county.CreatedAt <= now).IsTrue();
    }
  }

  [Test]
  public async Task UpdatedAt_Is_Altered(CancellationToken cancellationToken)
  {
    await using var ctx = await fixture.GetContextAsync("createdAt", new TimestampSaveChangesInterceptor());
    await ctx.Database.MigrateAsync(cancellationToken);
    var counties = CityDataFaker.Counties.Generate(10);
    ctx.AddRange(counties);

    await ctx.SaveChangesAsync(cancellationToken);
    var then = DateTimeOffset.Now;

    foreach (var county in counties)
    {
      county.Name = "REDUCTED";
    }

    await ctx.SaveChangesAsync(cancellationToken);
    var now = DateTimeOffset.Now;

    foreach (var county in counties)
    {
      await Assert.That(county.UpdatedAt >= then).IsTrue();
      await Assert.That(county.CreatedAt < county.UpdatedAt).IsTrue();
      await Assert.That(county.UpdatedAt <= now).IsTrue();
    }
  }
}
