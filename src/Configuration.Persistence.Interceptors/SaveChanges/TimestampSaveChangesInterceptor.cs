namespace Kritikos.Configuration.Persistence.Interceptors.SaveChanges;

using System;
using System.Diagnostics.CodeAnalysis;
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
public class TimestampSaveChangesInterceptor : SaveChangesInterceptor
{
  #region Overrides of SaveChangesInterceptor

  /// <inheritdoc />
  [ExcludeFromCodeCoverage] // Handled in async method
  public override InterceptionResult<int> SavingChanges(
    DbContextEventData eventData,
    InterceptionResult<int> result)
  {
    ArgumentNullException.ThrowIfNull(eventData);

    StampEntries(eventData.Context!.ChangeTracker);
    return base.SavingChanges(eventData, result);
  }

  /// <inheritdoc />
  public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
    DbContextEventData eventData,
    InterceptionResult<int> result,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(eventData);

    StampEntries(eventData.Context!.ChangeTracker);

    return base.SavingChangesAsync(eventData, result, cancellationToken);
  }

  #endregion

  private static void StampEntries(ChangeTracker tracker)
  {
    var now = DateTime.UtcNow;

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
