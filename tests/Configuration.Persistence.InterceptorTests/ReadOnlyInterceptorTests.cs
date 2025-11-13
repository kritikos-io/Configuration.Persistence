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
    await ctx.Database.MigrateAsync(TestContext.Current.CancellationToken);
    var people = CityDataFaker.People.Generate(30);

    ctx.People.AddRange(people);
    await ctx.SaveChangesAsync(TestContext.Current.CancellationToken);

    people = await ctx.People.ToListAsync(TestContext.Current.CancellationToken);
    Assert.Empty(people);
  }

  [Fact]
  public async Task Ensure_read_only()
  {
    var people = CityDataFaker.People.Generate(30);
    await using var ctx = await fixture.GetContextAsync("readonly");
    await ctx.Database.MigrateAsync(TestContext.Current.CancellationToken);

    ctx.People.AddRange(people);
    await ctx.SaveChangesAsync(TestContext.Current.CancellationToken);

    await using var readOnly =
      await fixture.GetContextAsync("readonly", new ReadOnlyDbSaveChangesInterceptor());

    var newPeople = await ctx.People.ToListAsync(TestContext.Current.CancellationToken);
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

    await readOnly.SaveChangesAsync(TestContext.Current.CancellationToken);
    newPeople = await readOnly.People.ToListAsync(TestContext.Current.CancellationToken);

    Assert.All(newPeople, p =>
    {
      Assert.NotEmpty(p.FirstName);
      Assert.NotEmpty(p.LastName);
    });
  }
}
