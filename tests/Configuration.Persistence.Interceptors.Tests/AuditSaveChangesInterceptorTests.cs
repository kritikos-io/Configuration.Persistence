namespace Kritikos.Configuration.Persistence.Interceptors.Tests;

using System;
using System.Threading.Tasks;

using Kritikos.Configuration.Persistence.Interceptors.SaveChanges;
using Kritikos.Configuration.Persistence.TestKit;
using Kritikos.Samples.CityCensus;
using Kritikos.Samples.CityCensus.Services;

using Microsoft.EntityFrameworkCore;

[ClassDataSource<SampleDbContextFixture>(Shared = SharedType.PerClass)]
public class AuditSaveChangesInterceptorTests(SampleDbContextFixture fixture)
{
  private static readonly Guid Creator = Guid.Parse("1813b30a-a352-416e-adee-282362f7ba4e");
  private static readonly Guid Editor = Guid.Parse("364b3527-0282-4fc7-aafc-547f2c87f641");

  private readonly SampleDbContextFixture fixture = fixture;

  [Test]
  public async Task SaveChangesAsync_AddedEntity_PopulatesCreatedBy(CancellationToken cancellationToken)
  {
    await using var ctx = await fixture.GetContextAsync(
      "createdBy",
      new AuditSaveChangesInterceptor<Guid>(new DummyAuditProvider(() => Creator)));
    await ctx.Database.MigrateAsync(cancellationToken);
    var people = CityDataFaker.People.Generate(30);
    ctx.People.AddRange(people);

    await ctx.SaveChangesAsync(cancellationToken);
    foreach (var person in people)
    {
      await Assert.That(person.CreatedBy).IsEqualTo(Creator);
      await Assert.That(person.UpdatedBy).IsEqualTo(person.CreatedBy);
    }
  }

  [Test]
  public async Task SaveChangesAsync_ModifiedEntityWithDifferentAuditor_PopulatesUpdatedBy(CancellationToken cancellationToken)
  {
    await using var ctx = await fixture.GetContextAsync(
      "updatedBy",
      new AuditSaveChangesInterceptor<Guid>(new DummyAuditProvider(() => Creator)));
    await ctx.Database.MigrateAsync(cancellationToken);
    ctx.People.AddRange(CityDataFaker.People.Generate(10));
    await ctx.SaveChangesAsync(cancellationToken);

    await using var ctx2 = await fixture.GetContextAsync(
      "updatedBy",
      new AuditSaveChangesInterceptor<Guid>(new DummyAuditProvider(() => Editor)));

    var people = await ctx2.People.ToListAsync(cancellationToken);
    await Assert.That(people).IsNotEmpty();

    foreach (var person in people)
    {
      person.FirstName = "REDACTED";
    }

    await ctx2.SaveChangesAsync(cancellationToken);

    foreach (var person in people)
    {
      await Assert.That(person.CreatedBy).IsEqualTo(Creator);
      await Assert.That(person.UpdatedBy).IsEqualTo(Editor);
    }
  }

  [Test]
  public async Task SaveChanges_AddedEntity_PopulatesCreatedBy(CancellationToken cancellationToken)
  {
    await using var ctx = await fixture.GetContextAsync(
      "createdBySync",
      new AuditSaveChangesInterceptor<Guid>(new DummyAuditProvider(() => Creator)));
    await ctx.Database.MigrateAsync(cancellationToken);
    var people = CityDataFaker.People.Generate(10);
    ctx.People.AddRange(people);

    ctx.SaveChanges();

    foreach (var person in people)
    {
      await Assert.That(person.CreatedBy).IsEqualTo(Creator);
      await Assert.That(person.UpdatedBy).IsEqualTo(person.CreatedBy);
    }
  }

  [Test]
  public async Task Constructor_NullAuditorProvider_ThrowsArgumentNullException()
  {
    await Assert.That(() => new AuditSaveChangesInterceptor<Guid>(null!))
      .Throws<ArgumentNullException>();
  }

  [Test]
  public async Task SavingChangesAsync_NullEventData_ThrowsArgumentNullException()
  {
    var interceptor = new AuditSaveChangesInterceptor<Guid>(new DummyAuditProvider(() => Creator));

    await Assert.That(async () => await interceptor.SavingChangesAsync(null!, default))
      .Throws<ArgumentNullException>();
  }
}
