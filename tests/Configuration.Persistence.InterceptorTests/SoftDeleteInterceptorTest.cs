namespace Kritikos.Configuration.Persistence.InterceptorTests;

using Kritikos.Configuration.Persistence.Interceptors.SaveChanges;
using Kritikos.Configuration.PersistenceTests;
using Kritikos.Samples.CityCensus;

using Microsoft.EntityFrameworkCore;

[ClassDataSource<SampleDbContextFixture>(Shared = SharedType.PerClass)]
public class SoftDeleteInterceptorTest(SampleDbContextFixture fixture)
{
  private const int TotalPeople = 10;
  private const int DeletedPeople = 4;

  private readonly SampleDbContextFixture fixture = fixture;

  [Test]
  public async Task Soft_deleted_items_are_filtered(CancellationToken cancellationToken)
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
  public async Task Soft_deleted_items_are_persisted(CancellationToken cancellationToken)
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
}
