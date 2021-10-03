namespace Kritikos.Configuration.Persistence.Interceptors.SaveChanges
{
  using System.Diagnostics.CodeAnalysis;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;

  using Kritikos.Configuration.Persistence.Contracts.Behavioral;

  using Microsoft.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore.ChangeTracking;
  using Microsoft.EntityFrameworkCore.Diagnostics;

  public class SoftDeleteSaveChangesInterceptor : SaveChangesInterceptor
  {
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
      UpdateSoftDeleteStatus(eventData.Context.ChangeTracker);
      return base.SavingChanges(eventData, result);
    }

    /// <inheritdoc />
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
      DbContextEventData eventData,
      InterceptionResult<int> result,
      CancellationToken cancellationToken = default)
    {
      UpdateSoftDeleteStatus(eventData.Context.ChangeTracker);
      return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void UpdateSoftDeleteStatus(ChangeTracker tracker)
    {
      var deleted = tracker.Entries<ISoftDeletable>()
        .Where(x => x.State == EntityState.Modified)
        .Where(x => !x.Entity.IsDeleted)
        .ToList();

      foreach (var entry in deleted)
      {
        entry.Entity.IsDeleted = true;
        entry.State = EntityState.Modified;
      }
    }
  }
}
