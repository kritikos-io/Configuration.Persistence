namespace Kritikos.Configuration.Persistence.Interceptors.SaveChanges;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Kritikos.Configuration.Persistence.Contracts.Behavioral;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

/// <summary>
/// Rewrites deletions of <see cref="ISoftDeletable"/> entities into updates flagging them as deleted.
/// </summary>
/// <param name="timeProvider">The <see cref="TimeProvider"/> supplying <see cref="ISoftDeletable.DeletedAt"/>, defaulting to <see cref="TimeProvider.System"/>.</param>
/// <remarks>Every entity deleted by one save shares a single instant, read once per save.</remarks>
public class SoftDeleteSaveChangesInterceptor(TimeProvider? timeProvider = null) : SaveChangesInterceptor
{
  private readonly TimeProvider timeProvider = timeProvider ?? TimeProvider.System;

  /// <inheritdoc />
  public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
  {
    ArgumentNullException.ThrowIfNull(eventData);

    if (eventData.Context is { } context)
    {
      UpdateSoftDeleteStatus(context.ChangeTracker, timeProvider.GetUtcNow().UtcDateTime);
    }

    return base.SavingChanges(eventData, result);
  }

  /// <inheritdoc />
  public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
    DbContextEventData eventData,
    InterceptionResult<int> result,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(eventData);

    if (eventData.Context is { } context)
    {
      UpdateSoftDeleteStatus(context.ChangeTracker, timeProvider.GetUtcNow().UtcDateTime);
    }

    return base.SavingChangesAsync(eventData, result, cancellationToken);
  }

  private static void UpdateSoftDeleteStatus(ChangeTracker tracker, DateTime now)
  {
    var deleted = tracker.Entries<ISoftDeletable>()
      .Where(x => x.State == EntityState.Deleted)
      .Where(x => !x.Entity.IsDeleted)
      .ToList();

    foreach (var entry in deleted)
    {
      entry.Entity.IsDeleted = true;
      entry.Entity.DeletedAt = now;
      entry.State = EntityState.Modified;
    }
  }
}
