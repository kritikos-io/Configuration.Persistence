#pragma warning disable SA1402 // File may only contain a single type
namespace Kritikos.Configuration.Persistence.Interceptors.Tests;

using Kritikos.Configuration.Persistence.Contracts;
using Kritikos.Configuration.Persistence.Contracts.Behavioral;
using Kritikos.Configuration.Persistence.Entities;

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
