namespace Kritikos.Configuration.Persistence.Interceptors.Tests;

using Kritikos.Configuration.Persistence.Interceptors.SaveChanges;
using Kritikos.Configuration.Persistence.TestKit;
using Kritikos.Samples.CityCensus;

using Microsoft.EntityFrameworkCore;

[ClassDataSource<SampleDbContextFixture>(Shared = SharedType.PerClass)]
public class SoftDeleteInterceptorTests(SampleDbContextFixture fixture)
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
  public async Task SavingChangesAsync_NullEventData_ThrowsArgumentNullException()
  {
    var interceptor = new SoftDeleteSaveChangesInterceptor();

    await Assert.That(async () => await interceptor.SavingChangesAsync(null!, default))
      .Throws<ArgumentNullException>();
  }
}
