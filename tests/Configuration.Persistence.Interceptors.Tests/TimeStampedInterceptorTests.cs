namespace Kritikos.Configuration.Persistence.Interceptors.Tests;

using System;
using System.Threading.Tasks;

using Kritikos.Configuration.Persistence.Interceptors.SaveChanges;
using Kritikos.Configuration.Persistence.TestKit;
using Kritikos.Samples.CityCensus;

using Microsoft.EntityFrameworkCore;

// Both tests share the "createdAt" in-memory database, which SQLite keys globally by name,
// so they must not run concurrently.
[ClassDataSource<SampleDbContextFixture>(Shared = SharedType.PerClass)]
[NotInParallel]
public class TimeStampedInterceptorTests(SampleDbContextFixture fixture)
{
  private readonly SampleDbContextFixture fixture = fixture;

  [Test]
  public async Task SaveChangesAsync_AddedEntity_PopulatesCreatedAt(CancellationToken cancellationToken)
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
  public async Task SaveChangesAsync_ModifiedEntity_AdvancesUpdatedAt(CancellationToken cancellationToken)
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

  [Test]
  public async Task SavingChangesAsync_NullEventData_ThrowsArgumentNullException()
  {
    var interceptor = new TimestampSaveChangesInterceptor();

    await Assert.That(async () => await interceptor.SavingChangesAsync(null!, default))
      .Throws<ArgumentNullException>();
  }
}
