namespace Kritikos.Configuration.Persistence.Interceptors.Tests;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Kritikos.Configuration.Persistence.Entities;
using Kritikos.Configuration.Persistence.Interceptors.SaveChanges;
using Kritikos.Configuration.Persistence.TestKit;
using Kritikos.Samples.CityCensus;

using Microsoft.EntityFrameworkCore;

[ClassDataSource<SampleDbContextFixture>(Shared = SharedType.PerClass)]
public class SharedInterceptorProbe(SampleDbContextFixture fixture)
{
  [Test]
  public async Task FailedSaveThenSuccessfulSave(CancellationToken cancellationToken)
  {
    await using var ctx = await fixture.GetContextAsync(
      "probe_failed",
      new AuditTrailSaveChangesInterceptor<AuditRecord, CityCensusTrailDbContext>());
    await ctx.Database.MigrateAsync(cancellationToken);

    var orphan = CityDataFaker.People.Generate(1)[0];
    ctx.People.Add(orphan);
    ctx.Entry(orphan).Property("CountyId").CurrentValue = 999999L;

    var failed = false;
    try
    {
      await ctx.SaveChangesAsync(cancellationToken);
    }
    catch (DbUpdateException)
    {
      failed = true;
    }

    ctx.ChangeTracker.Clear();

    ctx.Counties.AddRange(CityDataFaker.Counties.Generate(2));
    await ctx.SaveChangesAsync(cancellationToken);

    var records = await ctx.AuditRecords.ToListAsync(cancellationToken);
    var tables = string.Join(",", records.Select(x => x.Table).OrderBy(x => x));

    await Assert.That($"failed={failed} records={records.Count} tables={tables}").IsEqualTo("SHOW ME");
  }
}
