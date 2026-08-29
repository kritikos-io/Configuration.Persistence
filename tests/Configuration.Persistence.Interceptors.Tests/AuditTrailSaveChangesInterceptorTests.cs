namespace Kritikos.Configuration.Persistence.Interceptors.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Kritikos.Configuration.Persistence.Entities;
using Kritikos.Configuration.Persistence.Interceptors.SaveChanges;
using Kritikos.Configuration.Persistence.TestKit;
using Kritikos.Samples.CityCensus;
using Kritikos.Samples.CityCensus.Model;

using Microsoft.EntityFrameworkCore;

[ClassDataSource<SampleDbContextFixture>(Shared = SharedType.PerClass)]
public class AuditTrailSaveChangesInterceptorTests(SampleDbContextFixture fixture)
{
  private const int TotalCounties = 5;

  private readonly SampleDbContextFixture fixture = fixture;

  [Test]
  public async Task SaveChangesAsync_AddedEntities_RecordsOneTrailEntryPerEntity(CancellationToken cancellationToken)
  {
    await using var ctx = await fixture.GetContextAsync(
      "auditTrail_added",
      new AuditTrailSaveChangesInterceptor<AuditRecord, CityCensusTrailDbContext>());
    await ctx.Database.MigrateAsync(cancellationToken);
    ctx.Counties.AddRange(CityDataFaker.Counties.Generate(TotalCounties));

    await ctx.SaveChangesAsync(cancellationToken);

    var records = await ctx.AuditRecords.ToListAsync(cancellationToken);
    await Assert.That(records.Count).IsEqualTo(TotalCounties);
    await Assert.That(records.All(x => x.Modification == EntityState.Added)).IsTrue();
    await Assert.That(records.All(x => x.Table == "Counties")).IsTrue();
  }

  [Test]
  public async Task SaveChangesAsync_StoreGeneratedKeys_ResolvesTemporaryValuesAfterSaving(CancellationToken cancellationToken)
  {
    await using var ctx = await fixture.GetContextAsync(
      "auditTrail_keys",
      new AuditTrailSaveChangesInterceptor<AuditRecord, CityCensusTrailDbContext>());
    await ctx.Database.MigrateAsync(cancellationToken);
    var counties = CityDataFaker.Counties.Generate(TotalCounties);
    ctx.Counties.AddRange(counties);

    await ctx.SaveChangesAsync(cancellationToken);

    var keys = (await ctx.AuditRecords.ToListAsync(cancellationToken))
      .Select(x => Deserialize(x.Key))
      .ToList();
    await Assert.That(keys.All(x => x.ContainsKey(nameof(County.Id)))).IsTrue();
    await Assert.That(keys.Select(x => x[nameof(County.Id)].GetInt64()).ToList())
      .IsEquivalentTo(counties.Select(x => x.Id).ToList());
  }

  [Test]
  public async Task SaveChangesAsync_ModifiedEntity_RecordsBothOldAndNewValues(CancellationToken cancellationToken)
  {
    await using var ctx = await fixture.GetContextAsync(
      "auditTrail_modified",
      new AuditTrailSaveChangesInterceptor<AuditRecord, CityCensusTrailDbContext>());
    await ctx.Database.MigrateAsync(cancellationToken);
    var county = CityDataFaker.Counties.Generate(1)[0];
    var original = county.Name;
    ctx.Counties.Add(county);
    await ctx.SaveChangesAsync(cancellationToken);

    county.Name = "REDACTED";
    await ctx.SaveChangesAsync(cancellationToken);

    var record = await ctx.AuditRecords
      .SingleAsync(x => x.Modification == EntityState.Modified, cancellationToken);
    await Assert.That(Deserialize(record.OldValues)[nameof(County.Name)].GetString()).IsEqualTo(original);
    await Assert.That(Deserialize(record.NewValues)[nameof(County.Name)].GetString()).IsEqualTo("REDACTED");
  }

  [Test]
  public async Task SaveChangesAsync_RecordUnchangedPropertiesDisabled_RecordsOnlyModifiedProperties(CancellationToken cancellationToken)
  {
    await using var ctx = await fixture.GetContextAsync(
      "auditTrail_delta",
      new AuditTrailSaveChangesInterceptor<AuditRecord, CityCensusTrailDbContext>(recordUnchangedProperties: false));
    await ctx.Database.MigrateAsync(cancellationToken);
    var county = CityDataFaker.Counties.Generate(1)[0];
    ctx.Counties.Add(county);
    await ctx.SaveChangesAsync(cancellationToken);

    county.Name = "REDACTED";
    await ctx.SaveChangesAsync(cancellationToken);

    var record = await ctx.AuditRecords
      .SingleAsync(x => x.Modification == EntityState.Modified, cancellationToken);
    await Assert.That(Deserialize(record.NewValues).Keys.ToList()).IsEquivalentTo([nameof(County.Name)]);
  }

  [Test]
  public async Task SaveChangesAsync_DeletedEntity_RecordsPreviousValuesOnly(CancellationToken cancellationToken)
  {
    await using var ctx = await fixture.GetContextAsync(
      "auditTrail_deleted",
      new AuditTrailSaveChangesInterceptor<AuditRecord, CityCensusTrailDbContext>());
    await ctx.Database.MigrateAsync(cancellationToken);
    var county = CityDataFaker.Counties.Generate(1)[0];
    ctx.Counties.Add(county);
    await ctx.SaveChangesAsync(cancellationToken);
    var original = county.Name;

    ctx.Counties.Remove(county);
    await ctx.SaveChangesAsync(cancellationToken);

    var record = await ctx.AuditRecords
      .SingleAsync(x => x.Modification == EntityState.Deleted, cancellationToken);
    await Assert.That(Deserialize(record.OldValues)[nameof(County.Name)].GetString()).IsEqualTo(original);
    await Assert.That(Deserialize(record.NewValues)).IsEmpty();
  }

  [Test]
  public async Task Constructor_TraceableAuditRecord_ThrowsNotSupportedException()
    => await Assert.That(() =>
      {
        _ = new AuditTrailSaveChangesInterceptor<TraceableAuditRecord, TraceableAuditDbContext>();
      })
      .Throws<NotSupportedException>();

  [Test]
  public async Task Constructor_NullContext_ThrowsArgumentNullException()
    => await Assert.That(() =>
      {
        _ = new AuditTrailSaveChangesInterceptor<AuditRecord, CityCensusTrailDbContext>(null!);
      })
      .Throws<ArgumentNullException>();

  [Test]
  public async Task SaveChanges_AddedEntities_RecordsOneTrailEntryPerEntity(CancellationToken cancellationToken)
  {
    await using var ctx = await fixture.GetContextAsync(
      "auditTrail_sync",
      new AuditTrailSaveChangesInterceptor<AuditRecord, CityCensusTrailDbContext>());
    await ctx.Database.MigrateAsync(cancellationToken);
    ctx.Counties.AddRange(CityDataFaker.Counties.Generate(TotalCounties));

    ctx.SaveChanges();

    var records = await ctx.AuditRecords.ToListAsync(cancellationToken);
    await Assert.That(records.Count).IsEqualTo(TotalCounties);
    await Assert.That(records.All(x => x.CreatedAt != default)).IsTrue();
  }

  [Test]
  public async Task SaveChangesAsync_SavingContextIsNotTheTrailContext_ThrowsInvalidOperationException(
    CancellationToken cancellationToken)
  {
    await using var ctx = await fixture.GetContextAsync(
      "auditTrail_unresolved",
      new AuditTrailSaveChangesInterceptor<AuditRecord, UnrelatedAuditDbContext>());
    await ctx.Database.MigrateAsync(cancellationToken);
    ctx.Counties.AddRange(CityDataFaker.Counties.Generate(TotalCounties));

    await Assert.That(async () => await ctx.SaveChangesAsync(cancellationToken))
      .Throws<InvalidOperationException>();
  }

  [Test]
  public async Task SavingChangesAsync_NullEventData_ThrowsArgumentNullException()
  {
    var interceptor = new AuditTrailSaveChangesInterceptor<AuditRecord, CityCensusTrailDbContext>();

    await Assert.That(async () => await interceptor.SavingChangesAsync(null!, default))
      .Throws<ArgumentNullException>();
  }

  [Test]
  public async Task SaveChangesAsync_InstanceSharedBetweenContexts_WritesIntoTheSavingContext(
    CancellationToken cancellationToken)
  {
    var interceptor = new AuditTrailSaveChangesInterceptor<AuditRecord, CityCensusTrailDbContext>();

    await using var first = await fixture.GetContextAsync("auditTrail_shared_first", interceptor);
    await first.Database.MigrateAsync(cancellationToken);
    first.Counties.AddRange(CityDataFaker.Counties.Generate(TotalCounties));
    await first.SaveChangesAsync(cancellationToken);

    await using var second = await fixture.GetContextAsync("auditTrail_shared_second", interceptor);
    await second.Database.MigrateAsync(cancellationToken);
    second.Counties.AddRange(CityDataFaker.Counties.Generate(TotalCounties));
    await second.SaveChangesAsync(cancellationToken);

    await Assert.That(await first.AuditRecords.CountAsync(cancellationToken)).IsEqualTo(TotalCounties);
    await Assert.That(await second.AuditRecords.CountAsync(cancellationToken)).IsEqualTo(TotalCounties);
  }

  private static Dictionary<string, JsonElement> Deserialize(string json)
    => JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json) ?? [];
}
