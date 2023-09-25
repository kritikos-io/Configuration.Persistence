namespace Kritikos.Configuration.Persistence.InterceptorTests;

using Kritikos.Configuration.Persistence.Interceptors.SaveChanges;
using Kritikos.Configuration.PersistenceTests;
using Kritikos.Samples.CityCensus;

using Microsoft.EntityFrameworkCore;

using Xunit;

public class SoftDeleteInterceptorTest(SampleDbContextFixture fixture)
  : IClassFixture<SampleDbContextFixture>
{
  private const int TotalPeople = 10;
  private const int DeletedPeople = 4;

  [Fact]
  public async Task Soft_deleted_items_are_filtered()
  {
    await using var context =
      await fixture.GetContextAsync("softDelete_filter", new SoftDeleteSaveChangesInterceptor());
    await context.Database.MigrateAsync();
    var people = CityDataFaker.People.Generate(TotalPeople);
    context.People.AddRange(people);

    await context.SaveChangesAsync();
    context.People.RemoveRange(people.Take(DeletedPeople));
    await context.SaveChangesAsync();

    people = await context.People.ToListAsync();
    Assert.Equal(TotalPeople - DeletedPeople, people.Count);
  }

  [Fact]
  public async Task Soft_deleted_items_are_persisted()
  {
    await using var context =
      await fixture.GetContextAsync("softDelete_persist", new SoftDeleteSaveChangesInterceptor());
    await context.Database.MigrateAsync();
    var people = CityDataFaker.People.Generate(TotalPeople);
    context.People.AddRange(people);

    await context.SaveChangesAsync();
    context.People.RemoveRange(people.Take(DeletedPeople));
    await context.SaveChangesAsync();

    people = await context.People.IgnoreQueryFilters()
      .ToListAsync();
    Assert.Equal(TotalPeople, people.Count);
    Assert.Equal(DeletedPeople, people.Count(p => p.IsDeleted));
  }
}
