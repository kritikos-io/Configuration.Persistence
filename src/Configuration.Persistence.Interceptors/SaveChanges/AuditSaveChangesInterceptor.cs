namespace Kritikos.Configuration.Persistence.Interceptors.SaveChanges;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Kritikos.Configuration.Persistence.Contracts.Behavioral;
using Kritikos.Configuration.Persistence.Interceptors.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

/// <summary>
/// Populates audit values for <see cref="IAuditable{T}"/>, <see cref="ICreateAuditable{T}"/> and <see cref="IUpdateAuditable{T}"/> entities.
/// </summary>
/// <typeparam name="T">Type of audit field.</typeparam>
public class AuditSaveChangesInterceptor<T>(IAuditorProvider<T> auditorProvider)
  : SaveChangesInterceptor
  where T : IComparable, IComparable<T>, IEquatable<T>
{
  /// <inheritdoc />
  [ExcludeFromCodeCoverage] // Handled in async method
  public override InterceptionResult<int> SavingChanges(
    DbContextEventData eventData,
    InterceptionResult<int> result)
  {
    ArgumentNullException.ThrowIfNull(eventData);

    var auditor = auditorProvider.GetAuditor() ?? auditorProvider.GetFallbackAuditor();
    StampEntities(eventData.Context!.ChangeTracker, auditor);

    return base.SavingChanges(eventData, result);
  }

  /// <inheritdoc />
  public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
    DbContextEventData eventData,
    InterceptionResult<int> result,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(eventData);

    var auditor = auditorProvider.GetAuditor() ?? auditorProvider.GetFallbackAuditor();
    StampEntities(eventData.Context!.ChangeTracker, auditor);

    return base.SavingChangesAsync(eventData, result, cancellationToken);
  }

  private static void StampEntities(ChangeTracker tracker, T auditor)
  {
    var created = tracker.Entries<ICreateAuditable<T>>().Where(x => x.State == EntityState.Added).ToList();
    foreach (var x in created)
    {
      x.Entity.CreatedBy = auditor;
    }

    var updated = tracker.Entries<IUpdateAuditable<T>>()
      .Where(x => x.State is EntityState.Added or EntityState.Modified)
      .ToList();
    foreach (var x in updated)
    {
      x.Entity.UpdatedBy = auditor;
    }
  }
}
