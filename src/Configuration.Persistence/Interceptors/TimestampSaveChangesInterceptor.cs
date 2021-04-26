namespace Kritikos.Configuration.Persistence.Interceptors
{
  using System;
  using System.Diagnostics.CodeAnalysis;
  using System.Threading;
  using System.Threading.Tasks;

  using Kritikos.Configuration.Persistence.Contracts.Behavioral;

  using Microsoft.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore.Diagnostics;

  /// <summary>
  /// Populates timestamp values for <see cref="ITimestamped"/> entities.
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
      var entries = eventData.Context.ChangeTracker.Entries<ITimestamped>()
                    ?? throw new ArgumentNullException(nameof(eventData));
      var now = DateTimeOffset.Now;

      foreach (var entry in entries)
      {
        if (entry.State == EntityState.Added)
        {
          entry.Entity.CreatedAt = now;
        }

        entry.Entity.UpdatedAt = now;
      }

      return base.SavingChanges(eventData, result);
    }

    /// <inheritdoc />
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
      DbContextEventData eventData,
      InterceptionResult<int> result,
      CancellationToken cancellationToken = default)
    {
      var entries = eventData.Context.ChangeTracker.Entries<ITimestamped>()
                    ?? throw new ArgumentNullException(nameof(eventData));
      var now = DateTimeOffset.Now;

      foreach (var entry in entries)
      {
        if (entry.State == EntityState.Added)
        {
          entry.Entity.CreatedAt = now;
        }

        entry.Entity.UpdatedAt = now;
      }

      return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    #endregion
  }
}
