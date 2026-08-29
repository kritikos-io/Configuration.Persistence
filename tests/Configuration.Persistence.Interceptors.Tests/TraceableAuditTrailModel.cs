#pragma warning disable SA1402 // File may only contain a single type
namespace Kritikos.Configuration.Persistence.Interceptors.Tests;

using System;

using Kritikos.Configuration.Persistence.Contracts;
using Kritikos.Configuration.Persistence.Contracts.Behavioral;
using Kritikos.Configuration.Persistence.Entities;
using Kritikos.Configuration.Persistence.Extensions;

using Microsoft.EntityFrameworkCore;

// Only exists to prove the interceptor rejects audit records that would recursively audit themselves.
public class TraceableAuditRecord : AuditRecord, ITraceableAudit;

public class TraceableAuditDbContext : DbContext, IAuditTrailDbContext<TraceableAuditRecord>
{
  public DbSet<TraceableAuditRecord> AuditRecords => Set<TraceableAuditRecord>();
}

// Only exists to prove the interceptor rejects a saving context that is not the requested trail context.
public class UnrelatedAuditDbContext : DbContext, IAuditTrailDbContext<AuditRecord>
{
  public DbSet<AuditRecord> AuditRecords => Set<AuditRecord>();
}

// Only exists to prove an excluded key still reaches the trail.
public class ExcludedKeyEntity : ITraceableAudit
{
  public long Id { get; set; }

  public string Name { get; set; } = string.Empty;
}

public class ExcludedKeyDbContext(DbContextOptions<ExcludedKeyDbContext> options)
  : DbContext(options), IAuditTrailDbContext<AuditRecord>
{
  public DbSet<AuditRecord> AuditRecords => Set<AuditRecord>();

  public DbSet<ExcludedKeyEntity> Entities => Set<ExcludedKeyEntity>();

  /// <inheritdoc />
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    ArgumentNullException.ThrowIfNull(modelBuilder);
    base.OnModelCreating(modelBuilder);

    AuditRecord.OnModelCreating(modelBuilder);

    modelBuilder.Entity<ExcludedKeyEntity>(entity =>
    {
      entity.Property(e => e.Id).ExcludeFromAuditTrail();
      entity.Property(e => e.Name).ExcludeFromAuditTrail();
    });
  }
}
