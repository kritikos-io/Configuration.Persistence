namespace Kritikos.Configuration.Persistence.InterceptorTests;

using System.Threading.Tasks;

using Kritikos.Configuration.Persistence.Interceptors.SaveChanges;
using Kritikos.Configuration.PersistenceTests;
using Kritikos.Samples.CityCensus;

using Microsoft.EntityFrameworkCore;

using Xunit;

public class ReadOnlyInterceptorTests(SampleDbContextFixture fixture)
  : IClassFixture<SampleDbContextFixture>
{
  [Fact]
  public async Task Ensure_Database_is_unwritable()
  {
    await using var ctx = await fixture.GetContextAsync("readonly_db", new ReadOnlyDbSaveChangesInterceptor());
    await ctx.Database.MigrateAsync();
    var people = CityDataFaker.People.Generate(30);

    ctx.People.AddRange(people);
    await ctx.SaveChangesAsync();

    people = await ctx.People.ToListAsync();
    Assert.Empty(people);
  }

  [Fact]
  public async Task Ensure_read_only()
  {
    var people = CityDataFaker.People.Generate(30);
    await using var ctx = await fixture.GetContextAsync("readonly");
    await ctx.Database.MigrateAsync();

    ctx.People.AddRange(people);
    await ctx.SaveChangesAsync();

    await using var readOnly =
      await fixture.GetContextAsync("readonly", new ReadOnlyDbSaveChangesInterceptor());

    var newPeople = await ctx.People.ToListAsync();
    foreach (var person in newPeople)
    {
      person.FirstName = string.Empty;
      person.LastName = string.Empty;
    }

    Assert.All(newPeople, p =>
    {
      Assert.Empty(p.FirstName);
      Assert.Empty(p.LastName);
    });

    await readOnly.SaveChangesAsync();
    newPeople = await readOnly.People.ToListAsync();

    Assert.All(newPeople, p =>
    {
      Assert.NotEmpty(p.FirstName);
      Assert.NotEmpty(p.LastName);
    });
  }
}
