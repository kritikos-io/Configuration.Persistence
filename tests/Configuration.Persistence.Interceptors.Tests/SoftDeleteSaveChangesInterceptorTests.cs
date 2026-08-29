namespace Kritikos.Configuration.Persistence.Interceptors.Tests;

using Kritikos.Configuration.Persistence.Interceptors.SaveChanges;
using Kritikos.Configuration.Persistence.TestKit;
using Kritikos.Samples.CityCensus;
using Kritikos.Samples.CityCensus.Services;

using Microsoft.EntityFrameworkCore;

[ClassDataSource<SampleDbContextFixture>(Shared = SharedType.PerClass)]
public class SoftDeleteSaveChangesInterceptorTests(SampleDbContextFixture fixture)
{
  private const int TotalPeople = 10;
  private const int DeletedPeople = 4;

  private readonly SampleDbContextFixture fixture = fixture;

  [Test]
  public async Task SaveChangesAsync_RemovedEntities_AreExcludedFromQueries(CancellationToken cancellationToken)
  {
    await using var context =
      await fixture.GetContextAsync("softDelete_filter", new SoftDeleteSaveChangesInterceptor());
    await context.Database.MigrateAsync(cancellationToken);
    var people = CityDataFaker.People.Generate(TotalPeople);
    context.People.AddRange(people);

    await context.SaveChangesAsync(cancellationToken);
    context.People.RemoveRange(people.Take(DeletedPeople));
    await context.SaveChangesAsync(cancellationToken);

    people = await context.People.ToListAsync(cancellationToken);
    await Assert.That(people.Count).IsEqualTo(TotalPeople - DeletedPeople);
  }

  [Test]
  public async Task SaveChangesAsync_RemovedEntities_RemainInStoreFlaggedAsDeleted(CancellationToken cancellationToken)
  {
    await using var context =
      await fixture.GetContextAsync("softDelete_persist", new SoftDeleteSaveChangesInterceptor());
    await context.Database.MigrateAsync(cancellationToken);
    var people = CityDataFaker.People.Generate(TotalPeople);
    context.People.AddRange(people);

    await context.SaveChangesAsync(cancellationToken);
    context.People.RemoveRange(people.Take(DeletedPeople));
    await context.SaveChangesAsync(cancellationToken);

    people = await context.People.IgnoreQueryFilters()
      .ToListAsync(cancellationToken);
    await Assert.That(people.Count).IsEqualTo(TotalPeople);
    await Assert.That(people.Count(p => p.IsDeleted)).IsEqualTo(DeletedPeople);
  }

  [Test]
  public async Task SaveChanges_RemovedEntities_RemainInStoreFlaggedAsDeleted(CancellationToken cancellationToken)
  {
    await using var context =
      await fixture.GetContextAsync("softDelete_sync", new SoftDeleteSaveChangesInterceptor());
    await context.Database.MigrateAsync(cancellationToken);
    var people = CityDataFaker.People.Generate(TotalPeople);
    context.People.AddRange(people);

    context.SaveChanges();
    context.People.RemoveRange(people.Take(DeletedPeople));
    context.SaveChanges();

    people = await context.People.IgnoreQueryFilters()
      .ToListAsync(cancellationToken);
    await Assert.That(people.Count).IsEqualTo(TotalPeople);
    await Assert.That(people.Count(p => p.IsDeleted)).IsEqualTo(DeletedPeople);
  }

  [Test]
  public async Task SavingChangesAsync_NullEventData_ThrowsArgumentNullException()
  {
    var interceptor = new SoftDeleteSaveChangesInterceptor();

    await Assert.That(async () => await interceptor.SavingChangesAsync(null!, default))
      .Throws<ArgumentNullException>();
  }

  [Test]
  public async Task SaveChangesAsync_CustomTimeProvider_StampsTheSuppliedInstant(CancellationToken cancellationToken)
  {
    var clock = new FixedTimeProvider(new DateTimeOffset(2020, 1, 2, 3, 4, 5, TimeSpan.Zero));
    await using var context =
      await fixture.GetContextAsync("softDelete_clock", new SoftDeleteSaveChangesInterceptor(clock));
    await context.Database.MigrateAsync(cancellationToken);
    var people = CityDataFaker.People.Generate(TotalPeople);
    context.People.AddRange(people);

    await context.SaveChangesAsync(cancellationToken);
    context.People.RemoveRange(people.Take(DeletedPeople));
    await context.SaveChangesAsync(cancellationToken);

    var deleted = await context.People.IgnoreQueryFilters()
      .Where(p => p.IsDeleted)
      .ToListAsync(cancellationToken);
    await Assert.That(deleted.Count).IsEqualTo(DeletedPeople);
    foreach (var person in deleted)
    {
      await Assert.That(person.DeletedAt).IsEqualTo(clock.UtcNow.UtcDateTime);
    }
  }

  [Test]
  public async Task SaveChangesAsync_RegisteredBeforeAuditing_AttributesTheSoftDelete(CancellationToken cancellationToken)
  {
    // Soft deletion turns Deleted into Modified, which is the state the auditing interceptor filters on,
    // so registering it second is what lets a soft delete be attributed at all.
    var creator = Guid.Parse("1813b30a-a352-416e-adee-282362f7ba4e");
    var deleter = Guid.Parse("364b3527-0282-4fc7-aafc-547f2c87f641");
    var auditor = creator;
    await using var context = await fixture.GetContextAsync(
      "softDelete_ordering",
      new SoftDeleteSaveChangesInterceptor(),
      new AuditSaveChangesInterceptor<Guid>(new DummyAuditProvider(() => auditor)));
    await context.Database.MigrateAsync(cancellationToken);
    var person = CityDataFaker.People.Generate(1)[0];
    context.People.Add(person);
    await context.SaveChangesAsync(cancellationToken);

    auditor = deleter;
    context.People.Remove(person);
    await context.SaveChangesAsync(cancellationToken);

    await Assert.That(person.IsDeleted).IsTrue();
    await Assert.That(person.UpdatedBy).IsEqualTo(deleter);
  }
}
