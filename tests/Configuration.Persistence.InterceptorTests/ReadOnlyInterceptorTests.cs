namespace Kritikos.Configuration.Persistence.InterceptorTests;

using System.Threading.Tasks;

using Kritikos.Configuration.Persistence.Interceptors.SaveChanges;
using Kritikos.Configuration.PersistenceTests;
using Kritikos.Samples.CityCensus;

using Microsoft.EntityFrameworkCore;

[ClassDataSource<SampleDbContextFixture>(Shared = SharedType.PerClass)]
public class ReadOnlyInterceptorTests(SampleDbContextFixture fixture)
{
  [Test]
  public async Task Ensure_Database_is_unwritable(CancellationToken cancellationToken)
  {
    await using var ctx = await fixture.GetContextAsync("readonly_db", new ReadOnlyDbSaveChangesInterceptor());
    await ctx.Database.MigrateAsync(cancellationToken);
    var people = CityDataFaker.People.Generate(30);

    ctx.People.AddRange(people);
    await ctx.SaveChangesAsync(cancellationToken);

    people = await ctx.People.ToListAsync(cancellationToken);
    await Assert.That(people).IsEmpty();
  }

  [Test]
  public async Task Ensure_read_only(CancellationToken cancellationToken)
  {
    var people = CityDataFaker.People.Generate(30);
    await using var ctx = await fixture.GetContextAsync("readonly");
    await ctx.Database.MigrateAsync(cancellationToken);

    ctx.People.AddRange(people);
    await ctx.SaveChangesAsync(cancellationToken);

    await using var readOnly =
      await fixture.GetContextAsync("readonly", new ReadOnlyDbSaveChangesInterceptor());

    var newPeople = await ctx.People.ToListAsync(cancellationToken);
    foreach (var person in newPeople)
    {
      person.FirstName = string.Empty;
      person.LastName = string.Empty;
    }

    foreach (var person in newPeople)
    {
      await Assert.That(person.FirstName).IsEmpty();
      await Assert.That(person.LastName).IsEmpty();
    }

    await readOnly.SaveChangesAsync(cancellationToken);
    newPeople = await readOnly.People.ToListAsync(cancellationToken);

    foreach (var person in newPeople)
    {
      await Assert.That(person.FirstName).IsNotEmpty();
      await Assert.That(person.LastName).IsNotEmpty();
    }
  }
}
