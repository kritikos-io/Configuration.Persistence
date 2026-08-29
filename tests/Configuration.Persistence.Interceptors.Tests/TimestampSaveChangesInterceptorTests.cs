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
public class TimestampSaveChangesInterceptorTests(SampleDbContextFixture fixture)
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
      county.Name = "REDACTED";
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
  public async Task SaveChanges_AddedEntity_PopulatesCreatedAt(CancellationToken cancellationToken)
  {
    await using var ctx = await fixture.GetContextAsync("createdAtSync", new TimestampSaveChangesInterceptor());
    await ctx.Database.MigrateAsync(cancellationToken);
    var counties = CityDataFaker.Counties.Generate(10);
    ctx.AddRange(counties);

    var then = DateTimeOffset.Now;
    ctx.SaveChanges();
    var now = DateTimeOffset.Now;

    foreach (var county in counties)
    {
      await Assert.That(county.CreatedAt >= then).IsTrue();
      await Assert.That(county.UpdatedAt).IsEqualTo(county.CreatedAt);
      await Assert.That(county.CreatedAt <= now).IsTrue();
    }
  }

  [Test]
  public async Task SavingChangesAsync_NullEventData_ThrowsArgumentNullException()
  {
    var interceptor = new TimestampSaveChangesInterceptor();

    await Assert.That(async () => await interceptor.SavingChangesAsync(null!, default))
      .Throws<ArgumentNullException>();
  }

  [Test]
  public async Task SaveChangesAsync_CustomTimeProvider_StampsTheSuppliedInstant(CancellationToken cancellationToken)
  {
    var clock = new FixedTimeProvider(new DateTimeOffset(2020, 1, 2, 3, 4, 5, TimeSpan.Zero));
    await using var ctx = await fixture.GetContextAsync("createdAtClock", new TimestampSaveChangesInterceptor(clock));
    await ctx.Database.MigrateAsync(cancellationToken);
    var counties = CityDataFaker.Counties.Generate(10);
    ctx.AddRange(counties);

    await ctx.SaveChangesAsync(cancellationToken);

    foreach (var county in counties)
    {
      await Assert.That(county.CreatedAt).IsEqualTo(clock.UtcNow.UtcDateTime);
      await Assert.That(county.UpdatedAt).IsEqualTo(clock.UtcNow.UtcDateTime);
    }

    clock.UtcNow = clock.UtcNow.AddHours(1);
    foreach (var county in counties)
    {
      county.Name = "REDACTED";
    }

    await ctx.SaveChangesAsync(cancellationToken);

    foreach (var county in counties)
    {
      await Assert.That(county.UpdatedAt).IsEqualTo(clock.UtcNow.UtcDateTime);
      await Assert.That(county.CreatedAt).IsEqualTo(clock.UtcNow.AddHours(-1).UtcDateTime);
    }
  }
}
