#pragma warning disable SA1402 // File may only contain a single type
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
using Kritikos.Samples.CityCensus.Services;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

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
  [Arguments(true)]
  [Arguments(false)]
  public async Task SaveChangesAsync_AuditingRegisteredEitherWay_AttributesTheTrailItself(
    bool trailFirst,
    CancellationToken cancellationToken)
  {
    // The trail persists through a save of its own, which re-enters the pipeline, so attribution does not depend on registration order.
    var auditor = Guid.Parse("364b3527-0282-4fc7-aafc-547f2c87f641");
    var trail = new AuditTrailSaveChangesInterceptor<AuditRecord, CityCensusTrailDbContext>();
    var audit = new AuditSaveChangesInterceptor<Guid>(new DummyAuditProvider(() => auditor));
    await using var ctx = await fixture.GetContextAsync(
      $"auditTrail_attributed_{trailFirst}",
      trailFirst ? [trail, audit] : [audit, trail]);
    await ctx.Database.MigrateAsync(cancellationToken);
    ctx.Counties.AddRange(CityDataFaker.Counties.Generate(TotalCounties));

    await ctx.SaveChangesAsync(cancellationToken);

    var records = await ctx.AuditRecords.ToListAsync(cancellationToken);
    await Assert.That(records).IsNotEmpty();
    await Assert.That(records.All(x => x.CreatedBy == auditor)).IsTrue();
  }

  [Test]
  public async Task SaveChangesAsync_TrailEntry_StoresTheModificationAsText(CancellationToken cancellationToken)
  {
    await using var ctx = await fixture.GetContextAsync(
      "auditTrail_modificationText",
      new AuditTrailSaveChangesInterceptor<AuditRecord, CityCensusTrailDbContext>());
    await ctx.Database.MigrateAsync(cancellationToken);
    var county = CityDataFaker.Counties.Generate(1)[0];
    ctx.Counties.Add(county);
    await ctx.SaveChangesAsync(cancellationToken);

    county.Name = "Renamed";
    await ctx.SaveChangesAsync(cancellationToken);

    var stored = await ctx.Database
      .SqlQuery<string>($"SELECT Modification FROM AuditRecords ORDER BY Id")
      .ToListAsync(cancellationToken);
    await Assert.That(stored).IsEquivalentTo([nameof(EntityState.Added), nameof(EntityState.Modified)]);
  }

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

  [Test]
  public async Task SaveChangesAsync_ClockSharedWithTimestamping_MatchesTheTimestampOfTheAuditedEntity(
    CancellationToken cancellationToken)
  {
    var clock = new FixedTimeProvider(new DateTimeOffset(2020, 1, 2, 3, 4, 5, TimeSpan.Zero));
    await using var ctx = await fixture.GetContextAsync(
      "auditTrail_clock",
      new TimestampSaveChangesInterceptor(clock),
      new AuditTrailSaveChangesInterceptor<AuditRecord, CityCensusTrailDbContext>(timeProvider: clock));
    await ctx.Database.MigrateAsync(cancellationToken);
    var counties = CityDataFaker.Counties.Generate(TotalCounties);
    ctx.Counties.AddRange(counties);

    await ctx.SaveChangesAsync(cancellationToken);

    var records = await ctx.AuditRecords.ToListAsync(cancellationToken);
    await Assert.That(records.Count).IsEqualTo(TotalCounties);
    await Assert.That(records.All(x => x.CreatedAt == clock.UtcNow.UtcDateTime)).IsTrue();
    await Assert.That(counties.All(x => x.CreatedAt == clock.UtcNow.UtcDateTime)).IsTrue();
  }

  [Test]
  public async Task SaveChangesAsync_SaveFailed_DiscardsTheEntriesHeldBackForIt(CancellationToken cancellationToken)
  {
    await using var ctx = await fixture.GetContextAsync(
      "auditTrail_failedAsync",
      new AuditTrailSaveChangesInterceptor<AuditRecord, CityCensusTrailDbContext>());
    await ctx.Database.MigrateAsync(cancellationToken);
    var county = CityDataFaker.Counties.Generate(1)[0];
    ctx.Counties.Add(county);
    await ctx.SaveChangesAsync(cancellationToken);

    Orphan(ctx);
    await Assert.That(async () => await ctx.SaveChangesAsync(cancellationToken)).Throws<DbUpdateException>();
    ctx.ChangeTracker.Clear();

    // Holds nothing back of its own, so anything the failed save left behind would be flushed here.
    var tracked = await ctx.Counties.SingleAsync(cancellationToken);
    tracked.Name = "Renamed";
    await ctx.SaveChangesAsync(cancellationToken);

    // The failed row was never written, so the trail must not claim it was.
    var tables = await ctx.AuditRecords.Select(x => x.Table).ToListAsync(cancellationToken);
    await Assert.That(tables).IsEquivalentTo(["Counties", "Counties"]);
  }

  [Test]
  public async Task SaveChanges_SaveFailed_DiscardsTheEntriesHeldBackForIt(CancellationToken cancellationToken)
  {
    await using var ctx = await fixture.GetContextAsync(
      "auditTrail_failedSync",
      new AuditTrailSaveChangesInterceptor<AuditRecord, CityCensusTrailDbContext>());
    await ctx.Database.MigrateAsync(cancellationToken);
    var county = CityDataFaker.Counties.Generate(1)[0];
    ctx.Counties.Add(county);
    ctx.SaveChanges();

    Orphan(ctx);
    await Assert.That(ctx.SaveChanges).Throws<DbUpdateException>();
    ctx.ChangeTracker.Clear();

    var tracked = await ctx.Counties.SingleAsync(cancellationToken);
    tracked.Name = "Renamed";
    ctx.SaveChanges();

    var tables = await ctx.AuditRecords.Select(x => x.Table).ToListAsync(cancellationToken);
    await Assert.That(tables).IsEquivalentTo(["Counties", "Counties"]);
  }

  [Test]
  public async Task SaveChangesAsync_InstanceSharedWithAFailingContext_KeepsItsOwnHeldBackEntries(
    CancellationToken cancellationToken)
  {
    // A single instance registered through AddDbContext serves every context, so one save must not disturb another.
    var shared = new AuditTrailSaveChangesInterceptor<AuditRecord, CityCensusTrailDbContext>();
    await using var other = await fixture.GetContextAsync("auditTrail_sharedOther", shared);
    await other.Database.MigrateAsync(cancellationToken);

    var interleaved = false;
    await using var ctx = await fixture.GetContextAsync(
      "auditTrail_sharedOwner",
      shared,
      new CallbackSaveChangesInterceptor(async () =>
      {
        if (interleaved)
        {
          return;
        }

        // Fails while the outer save is still holding its store generated keys back.
        interleaved = true;
        Orphan(other);
        try
        {
          await other.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
          other.ChangeTracker.Clear();
        }
      }));
    await ctx.Database.MigrateAsync(cancellationToken);
    var counties = CityDataFaker.Counties.Generate(TotalCounties);
    ctx.Counties.AddRange(counties);

    await ctx.SaveChangesAsync(cancellationToken);

    await Assert.That(interleaved).IsTrue();
    var keys = (await ctx.AuditRecords.ToListAsync(cancellationToken))
      .Select(x => Deserialize(x.Key))
      .ToList();
    await Assert.That(keys.Count).IsEqualTo(TotalCounties);
    await Assert.That(keys.Select(x => x[nameof(County.Id)].GetInt64()).ToList())
      .IsEquivalentTo(counties.Select(x => x.Id).ToList());
  }

  [Test]
  public async Task SaveChangesAsync_ExcludedPropertyOnAddedEntity_NamesItInsteadOfRecordingIt(
    CancellationToken cancellationToken)
  {
    await using var ctx = await fixture.GetContextAsync(
      "auditTrail_excludedAdded",
      new AuditTrailSaveChangesInterceptor<AuditRecord, CityCensusTrailDbContext>());
    await ctx.Database.MigrateAsync(cancellationToken);
    var person = CityDataFaker.People.Generate(1)[0];
    ctx.People.Add(person);

    await ctx.SaveChangesAsync(cancellationToken);

    var record = await ctx.AuditRecords.SingleAsync(cancellationToken);
    var values = Deserialize(record.NewValues);
    await Assert.That(values.ContainsKey(nameof(Person.Email))).IsFalse();
    await Assert.That(record.NewValues.Contains(person.Email, StringComparison.Ordinal)).IsFalse();
    await Assert.That(values[nameof(Person.FirstName)].GetString()).IsEqualTo(person.FirstName);
    await Assert.That(record.Redacted).IsEquivalentTo([nameof(Person.Email)]);
  }

  [Test]
  public async Task SaveChangesAsync_ExcludedPropertyModified_NamesItWithoutEitherValue(
    CancellationToken cancellationToken)
  {
    await using var ctx = await fixture.GetContextAsync(
      "auditTrail_excludedModified",
      new AuditTrailSaveChangesInterceptor<AuditRecord, CityCensusTrailDbContext>());
    await ctx.Database.MigrateAsync(cancellationToken);
    var person = CityDataFaker.People.Generate(1)[0];
    ctx.People.Add(person);
    await ctx.SaveChangesAsync(cancellationToken);
    var original = person.Email;

    person.Email = "moved@example.com";
    await ctx.SaveChangesAsync(cancellationToken);

    var record = await ctx.AuditRecords
      .SingleAsync(x => x.Modification == EntityState.Modified, cancellationToken);
    await Assert.That(Deserialize(record.OldValues).ContainsKey(nameof(Person.Email))).IsFalse();
    await Assert.That(Deserialize(record.NewValues).ContainsKey(nameof(Person.Email))).IsFalse();
    await Assert.That(record.OldValues.Contains(original, StringComparison.Ordinal)).IsFalse();
    await Assert.That(record.NewValues.Contains("moved@example.com", StringComparison.Ordinal)).IsFalse();
    await Assert.That(record.Redacted).IsEquivalentTo([nameof(Person.Email)]);
  }

  [Test]
  public async Task SaveChangesAsync_ExcludedPropertyUnchanged_LeavesItOutOfTheRedactedNames(
    CancellationToken cancellationToken)
  {
    await using var ctx = await fixture.GetContextAsync(
      "auditTrail_excludedUnchanged",
      new AuditTrailSaveChangesInterceptor<AuditRecord, CityCensusTrailDbContext>());
    await ctx.Database.MigrateAsync(cancellationToken);
    var person = CityDataFaker.People.Generate(1)[0];
    ctx.People.Add(person);
    await ctx.SaveChangesAsync(cancellationToken);

    person.FirstName = "Renamed";
    await ctx.SaveChangesAsync(cancellationToken);

    var record = await ctx.AuditRecords
      .SingleAsync(x => x.Modification == EntityState.Modified, cancellationToken);
    var values = Deserialize(record.NewValues);

    // Recorded despite being untouched, so recordUnchangedProperties is on and would have carried the address too.
    await Assert.That(values.ContainsKey(nameof(Person.LastName))).IsTrue();
    await Assert.That(values.ContainsKey(nameof(Person.Email))).IsFalse();
    await Assert.That(record.NewValues.Contains(person.Email, StringComparison.Ordinal)).IsFalse();
    await Assert.That(record.Redacted).IsEmpty();
  }

  [Test]
  public async Task SaveChangesAsync_ExcludedPropertyOnDeletedEntity_NamesItWithoutTheValue(
    CancellationToken cancellationToken)
  {
    await using var ctx = await fixture.GetContextAsync(
      "auditTrail_excludedDeleted",
      new AuditTrailSaveChangesInterceptor<AuditRecord, CityCensusTrailDbContext>());
    await ctx.Database.MigrateAsync(cancellationToken);
    var person = CityDataFaker.People.Generate(1)[0];
    ctx.People.Add(person);
    await ctx.SaveChangesAsync(cancellationToken);

    ctx.People.Remove(person);
    await ctx.SaveChangesAsync(cancellationToken);

    var record = await ctx.AuditRecords
      .SingleAsync(x => x.Modification == EntityState.Deleted, cancellationToken);
    await Assert.That(Deserialize(record.OldValues).ContainsKey(nameof(Person.Email))).IsFalse();
    await Assert.That(record.OldValues.Contains(person.Email, StringComparison.Ordinal)).IsFalse();
    await Assert.That(record.Redacted).IsEquivalentTo([nameof(Person.Email)]);
    await Assert.That(Deserialize(record.NewValues)).IsEmpty();
  }

  [Test]
  public async Task SaveChangesAsync_ExcludedPrimaryKey_KeepsItInTheKeyAnyway(CancellationToken cancellationToken)
  {
    await using var connection = new SqliteConnection("DataSource=auditTrail_excludedKey;mode=memory;cache=shared");
    await connection.OpenAsync(cancellationToken);
    var options = new DbContextOptionsBuilder<ExcludedKeyDbContext>()
      .UseSqlite(connection)
      .AddInterceptors(new AuditTrailSaveChangesInterceptor<AuditRecord, ExcludedKeyDbContext>())
      .Options;
    await using var ctx = new ExcludedKeyDbContext(options);
    await ctx.Database.EnsureCreatedAsync(cancellationToken);
    var entity = new ExcludedKeyEntity { Name = "secret" };
    ctx.Entities.Add(entity);

    await ctx.SaveChangesAsync(cancellationToken);

    var record = await ctx.AuditRecords.SingleAsync(cancellationToken);

    // A trail entry that cannot be traced back to a row is worthless, so the key outranks its exclusion.
    await Assert.That(Deserialize(record.Key)[nameof(ExcludedKeyEntity.Id)].GetInt64()).IsEqualTo(entity.Id);
    await Assert.That(record.NewValues.Contains("secret", StringComparison.Ordinal)).IsFalse();
    await Assert.That(record.Redacted).IsEquivalentTo([nameof(ExcludedKeyEntity.Name)]);
  }

  private static Dictionary<string, JsonElement> Deserialize(string json)
    => JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json) ?? [];

  private static void Orphan(CityCensusTrailDbContext context)
  {
    // Points at a county that does not exist, so the save fails on the foreign key.
    var person = CityDataFaker.People.Generate(1)[0];
    context.People.Add(person);
    context.Entry(person).Property("CountyId").CurrentValue = 999999L;
  }
}

internal sealed class CallbackSaveChangesInterceptor(Func<Task> onSaving) : SaveChangesInterceptor
{
  public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
    DbContextEventData eventData,
    InterceptionResult<int> result,
    CancellationToken cancellationToken = default)
  {
    await onSaving();

    return await base.SavingChangesAsync(eventData, result, cancellationToken);
  }
}
