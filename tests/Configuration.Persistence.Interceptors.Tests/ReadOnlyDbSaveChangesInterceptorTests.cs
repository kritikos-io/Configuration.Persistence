namespace Kritikos.Configuration.Persistence.Interceptors.Tests;

using System.Threading.Tasks;

using Kritikos.Configuration.Persistence.Interceptors.SaveChanges;
using Kritikos.Configuration.Persistence.TestKit;
using Kritikos.Samples.CityCensus;

using Microsoft.EntityFrameworkCore;

[ClassDataSource<SampleDbContextFixture>(Shared = SharedType.PerClass)]
public class ReadOnlyDbSaveChangesInterceptorTests(SampleDbContextFixture fixture)
{
  private readonly SampleDbContextFixture fixture = fixture;

  [Test]
  public async Task SaveChangesAsync_AddedEntities_AreNotPersisted(CancellationToken cancellationToken)
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
  public async Task SaveChanges_AddedEntities_AreNotPersisted(CancellationToken cancellationToken)
  {
    await using var ctx = await fixture.GetContextAsync("readonly_sync", new ReadOnlyDbSaveChangesInterceptor());
    await ctx.Database.MigrateAsync(cancellationToken);

    ctx.People.AddRange(CityDataFaker.People.Generate(10));
    var affected = ctx.SaveChanges();

    await Assert.That(affected).IsEqualTo(0);

    await using var verification = await fixture.GetContextAsync("readonly_sync");
    await Assert.That(await verification.People.ToListAsync(cancellationToken)).IsEmpty();
  }

  [Test]
  public async Task SaveChangesAsync_ModifiedExistingEntities_LeavesStoredValuesUnchanged(CancellationToken cancellationToken)
  {
    var people = CityDataFaker.People.Generate(30);
    await using var ctx = await fixture.GetContextAsync("readonly");
    await ctx.Database.MigrateAsync(cancellationToken);

    ctx.People.AddRange(people);
    await ctx.SaveChangesAsync(cancellationToken);

    await using var readOnly =
      await fixture.GetContextAsync("readonly", new ReadOnlyDbSaveChangesInterceptor());

    var tracked = await readOnly.People.ToListAsync(cancellationToken);
    await Assert.That(tracked).IsNotEmpty();

    foreach (var person in tracked)
    {
      person.FirstName = string.Empty;
      person.LastName = string.Empty;
    }

    await readOnly.SaveChangesAsync(cancellationToken);

    await using var verification = await fixture.GetContextAsync("readonly");
    var stored = await verification.People.ToListAsync(cancellationToken);
    await Assert.That(stored).IsNotEmpty();

    foreach (var person in stored)
    {
      await Assert.That(person.FirstName).IsNotEmpty();
      await Assert.That(person.LastName).IsNotEmpty();
    }
  }

  [Test]
  public async Task SavingChanges_NullEventData_ThrowsArgumentNullException()
  {
    var interceptor = new ReadOnlyDbSaveChangesInterceptor();

    await Assert.That(() => interceptor.SavingChanges(null!, default))
      .Throws<ArgumentNullException>();
  }

  [Test]
  public async Task SavingChangesAsync_NullEventData_ThrowsArgumentNullException()
  {
    var interceptor = new ReadOnlyDbSaveChangesInterceptor();

    await Assert.That(async () => await interceptor.SavingChangesAsync(null!, default))
      .Throws<ArgumentNullException>();
  }
}
