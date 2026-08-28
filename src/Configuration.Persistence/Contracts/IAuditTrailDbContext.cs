namespace Kritikos.Configuration.Persistence.Contracts;

using Kritikos.Configuration.Persistence.Entities;

using Microsoft.EntityFrameworkCore;

/// <summary>
/// Exposes Audit Record table allowing interceptors to enter entries.
/// </summary>
/// <typeparam name="TAudit">Type of Audit Record class.</typeparam>
public interface IAuditTrailDbContext<TAudit>
  where TAudit : AuditRecord
{
  /// <summary>
  /// Gets the table interceptors write audit trail entries to.
  /// </summary>
  DbSet<TAudit> AuditRecords { get; }
}
