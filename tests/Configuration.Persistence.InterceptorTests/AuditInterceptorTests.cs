namespace Kritikos.Configuration.Persistence.InterceptorTests;

using System;
using System.Threading.Tasks;

using Kritikos.Configuration.Persistence.Interceptors.SaveChanges;
using Kritikos.Configuration.PersistenceTests;
using Kritikos.Samples.CityCensus;
using Kritikos.Samples.CityCensus.Services;

using Microsoft.EntityFrameworkCore;

using Xunit;

public class AuditInterceptorTests(SampleDbContextFixture fixture)
  : IClassFixture<SampleDbContextFixture>
{
  private static readonly Guid Creator = Guid.Parse("1813b30a-a352-416e-adee-282362f7ba4e");
  private static readonly Guid Editor = Guid.Parse("364b3527-0282-4fc7-aafc-547f2c87f641");

  [Fact]
  public async Task CreatedBy_Is_Populated()
  {
    await using var ctx = await fixture.GetContextAsync(
      "createdBy",
      new AuditSaveChangesInterceptor<Guid>(new DummyAuditProvider(() => Creator)));
    await ctx.Database.MigrateAsync();
    var people = CityDataFaker.People.Generate(30);
    ctx.People.AddRange(people);

    await ctx.SaveChangesAsync();
    Assert.All(people, x =>
    {
      Assert.Equal(Creator, x.CreatedBy);
      Assert.Equal(x.CreatedBy, x.UpdatedBy);
    });
  }

  [Fact]
  public async Task UpdatedBy_Is_Populated()
  {
    await using var ctx = await fixture.GetContextAsync(
      "updatedBy",
      new AuditSaveChangesInterceptor<Guid>(new DummyAuditProvider(() => Creator)));
    await ctx.Database.MigrateAsync();
    await ctx.SaveChangesAsync();

    await using var ctx2 = await fixture.GetContextAsync(
      "updatedBy",
      new AuditSaveChangesInterceptor<Guid>(new DummyAuditProvider(() => Editor)));

    var people = await ctx2.People.ToListAsync();
    foreach (var person in people)
    {
      person.FirstName = "REDUCTED";
    }

    await ctx2.SaveChangesAsync();

    Assert.All(people, x => Assert.Equal(Editor, x.UpdatedBy));
  }
}
