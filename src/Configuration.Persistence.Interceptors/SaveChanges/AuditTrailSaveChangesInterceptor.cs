namespace Kritikos.Configuration.Persistence.Interceptors.SaveChanges
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text.Json;
  using System.Threading;
  using System.Threading.Tasks;

  using Kritikos.Configuration.Persistence.Contracts;
  using Kritikos.Configuration.Persistence.Contracts.Behavioral;
  using Kritikos.Configuration.Persistence.Entities;

  using Microsoft.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore.ChangeTracking;
  using Microsoft.EntityFrameworkCore.Diagnostics;

  /// <summary>
  /// Saves <typeparamref name="TAuditRecord"/> trails in the same <typeparamref name="TContext"/> or a different one as long as it implements <see cref="IAuditTrailDbContext{TAudit}"/>.
  /// Entities to be audited should implement <see cref="ITraceableAudit"/>.
  /// </summary>
  /// <typeparam name="TAuditRecord">Overridable entity that contains audit fields, should extend <see cref="AuditRecord"/>.</typeparam>
  /// <typeparam name="TContext">The type of the <see cref="DbContext"/> to save into.</typeparam>
  /// <remarks><typeparamref name="TAuditRecord"/> can not implement <see cref="ITraceableAudit"/>, otherwise <see cref="SavingChangesAsync"/> and/or <see cref="SavingChanges"/> would recursively call themselves.</remarks>
  /// <exception cref="NotSupportedException"><typeparamref name="TAuditRecord"/> extends <see cref="ITraceableAudit"/>.</exception>
  public class AuditTrailSaveChangesInterceptor<TAuditRecord, TContext> : SaveChangesInterceptor
    where TAuditRecord : AuditRecord, new()
    where TContext : DbContext, IAuditTrailDbContext<TAuditRecord>
  {
    private readonly bool recordUnchangedProperties;
    private TContext? context;
    private List<AuditEntry> transient = new();

    /// <summary>
    /// Creates a <see cref="AuditTrailSaveChangesInterceptor{TAuditRecord,TContext}"/> that will record audit trails in the same context as the actual entities.
    /// </summary>
    /// <param name="recordUnchangedProperties">If true, records only delta changes between states, otherwise records the complete current and previous state.</param>
    public AuditTrailSaveChangesInterceptor(bool recordUnchangedProperties = true)
    {
      if (typeof(ITraceableAudit).IsAssignableFrom(typeof(TAuditRecord)))
      {
        throw new NotSupportedException($"{typeof(TAuditRecord).Name} cannot be {nameof(ITraceableAudit)}.");
      }

      this.recordUnchangedProperties = recordUnchangedProperties;
    }

    /// <summary>
    /// Creates a <see cref="AuditTrailSaveChangesInterceptor{TAuditRecord,TContext}"/> that will record audit trails in the passed instance of <paramref name="context"/>.
    /// </summary>
    /// <param name="context">The <see cref="DbContext"/> to save <see cref="AuditRecord"/> to.</param>
    /// <param name="recordUnchangedProperties">If true, records only delta changes between states, otherwise records the complete current and previous state.</param>
    public AuditTrailSaveChangesInterceptor(TContext context, bool recordUnchangedProperties = true)
    {
      this.context = context;
      this.recordUnchangedProperties = recordUnchangedProperties;
    }

    #region Overrides of SaveChangesInterceptor

    /// <inheritdoc />
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
      context ??= eventData.Context as TContext;
      var entries = eventData.Context.ChangeTracker
        .Entries<ITraceableAudit>()
        .Where(x => x.State is not EntityState.Detached and not EntityState.Unchanged)
        .ToList();

      if (entries.Any())
      {
        transient = CreateAuditEntries(entries);
      }

      return base.SavingChanges(eventData, result);
    }

    /// <inheritdoc />
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
      DbContextEventData eventData,
      InterceptionResult<int> result,
      CancellationToken cancellationToken = default)
    {
      context ??= eventData.Context as TContext;
      var entries = eventData.Context.ChangeTracker
        .Entries<ITraceableAudit>()
        .Where(x => x.State is not EntityState.Detached and not EntityState.Unchanged)
        .ToList();

      if (entries.Any())
      {
        CreateAuditEntries(entries);
      }

      return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <inheritdoc />
    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
      var saveResult = base.SavedChanges(eventData, result);

      if (transient.Any())
      {
        UpdateTemporaryProperties(transient);
        transient.Clear();
        context!.SaveChanges();
      }

      return saveResult;
    }

    /// <inheritdoc />
    public override async ValueTask<int> SavedChangesAsync(
      SaveChangesCompletedEventData eventData,
      int result,
      CancellationToken cancellationToken = default)
    {
      var saveResult = await base.SavedChangesAsync(eventData, result, cancellationToken);

      if (transient.Any())
      {
        UpdateTemporaryProperties(transient);
        transient.Clear();
        await context!.SaveChangesAsync(cancellationToken);
      }

      return saveResult;
    }

    #endregion

    private List<AuditEntry> CreateAuditEntries(IReadOnlyCollection<EntityEntry<ITraceableAudit>> entries)
    {
      List<AuditEntry> auditEntries = new(entries.Count);
      foreach (var entry in entries)
      {
        var audit = new AuditEntry { TableName = entry.Metadata.GetTableName(), State = entry.State };
        auditEntries.Add(audit);
        foreach (var property in entry.Properties)
        {
          if (property.IsTemporary)
          {
            audit.TemporaryProperties.Add(property);
            continue;
          }

          if (property.Metadata.IsPrimaryKey())
          {
            audit.KeyValues.Add(property.Metadata.Name, property.CurrentValue);
            continue;
          }

          switch (entry.State)
          {
            case EntityState.Added:
              audit.NewValues.Add(property.Metadata.Name, property.CurrentValue);
              break;

            case EntityState.Deleted:
              audit.OldValues.Add(property.Metadata.Name, property.OriginalValue);
              break;

            case EntityState.Modified:
              if (recordUnchangedProperties || property.IsModified)
              {
                audit.NewValues.Add(property.Metadata.Name, property.CurrentValue);
                audit.OldValues.Add(property.Metadata.Name, property.OriginalValue);
              }

              break;
          }
        }
      }

      var permanent = auditEntries.Where(x => !x.TemporaryProperties.Any()).Select(x => x.ToAuditRecord()).ToList();
      context!.AuditRecords.AddRange(permanent);
      transient = auditEntries.Where(x => x.TemporaryProperties.Any()).ToList();

      return transient;
    }

    private void UpdateTemporaryProperties(List<AuditEntry> entries)
    {
      foreach (var entry in entries)
      {
        foreach (var property in entry.TemporaryProperties)
        {
          if (property.Metadata.IsPrimaryKey())
          {
            entry.KeyValues.Add(property.Metadata.Name, property.CurrentValue);
          }
          else
          {
            entry.NewValues.Add(property.Metadata.Name, property.CurrentValue);
          }
        }

        context!.AuditRecords.Add(entry.ToAuditRecord());
      }
    }

    private class AuditEntry
    {
      private const string Empty = "{}";

      public string TableName { get; init; } = string.Empty;

      public EntityState State { get; init; }

      public Dictionary<string, object> KeyValues { get; } = new();

      public Dictionary<string, object> OldValues { get; } = new();

      public Dictionary<string, object> NewValues { get; } = new();

      public List<PropertyEntry> TemporaryProperties { get; } = new();

      public TAuditRecord ToAuditRecord()
        => new()
        {
          Table = TableName,
          Key = JsonSerializer.Serialize(KeyValues),
          OldValues = OldValues.Any()
            ? JsonSerializer.Serialize(OldValues)
            : Empty,
          NewValues = NewValues.Any()
            ? JsonSerializer.Serialize(NewValues)
            : Empty,
          Modification = State,
        };
    }
  }
}
