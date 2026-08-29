namespace Kritikos.Configuration.Persistence.Interceptors.SaveChanges;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Kritikos.Configuration.Persistence.Contracts.Behavioral;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

/// <summary>
/// Populates timestamp values for <see cref="ITimestamped"/>, <see cref="ICreateTimestamped"/> and <see cref="IUpdateTimestamped"/> entities.
/// </summary>
/// <param name="timeProvider">The <see cref="TimeProvider"/> supplying the timestamp, defaulting to <see cref="TimeProvider.System"/>.</param>
/// <remarks>Every entity stamped by one save shares a single instant, read once per save.</remarks>
public class TimestampSaveChangesInterceptor(TimeProvider? timeProvider = null) : SaveChangesInterceptor
{
  private readonly TimeProvider timeProvider = timeProvider ?? TimeProvider.System;

  /// <inheritdoc />
  public override InterceptionResult<int> SavingChanges(
    DbContextEventData eventData,
    InterceptionResult<int> result)
  {
    ArgumentNullException.ThrowIfNull(eventData);

    if (eventData.Context is { } context)
    {
      StampEntries(context.ChangeTracker, timeProvider.GetUtcNow().UtcDateTime);
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
      StampEntries(context.ChangeTracker, timeProvider.GetUtcNow().UtcDateTime);
    }

    return base.SavingChangesAsync(eventData, result, cancellationToken);
  }

  private static void StampEntries(ChangeTracker tracker, DateTime now)
  {
    var created = tracker.Entries<ICreateTimestamped>()
      .Where(x => x.State == EntityState.Added)
      .ToList();
    foreach (var x in created)
    {
      x.Entity.CreatedAt = now;
    }

    var updated = tracker.Entries<IUpdateTimestamped>()
      .Where(x => x.State is EntityState.Added or EntityState.Modified)
      .ToList();
    foreach (var x in updated)
    {
      x.Entity.UpdatedAt = now;
    }
  }
}
