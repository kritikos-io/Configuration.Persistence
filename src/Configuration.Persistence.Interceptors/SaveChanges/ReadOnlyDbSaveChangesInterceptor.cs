namespace Kritikos.Configuration.Persistence.Interceptors.SaveChanges;

using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

/// <summary>
/// Makes a <see cref="DbContext"/> readonly by intercepting and preventing any attempt to save changes.
/// </summary>
/// <remarks>
/// Adding this class to the list of interceptors for an instance of a <see cref="DbContext"/> is final, consider using <see cref="QueryTrackingBehavior"/> for a runtime mutable behavior.
/// Suppressed saves report success and leave the change tracker dirty, so pending changes are resubmitted by every subsequent save on the same context.
/// </remarks>
public class ReadOnlyDbSaveChangesInterceptor : SaveChangesInterceptor
{
  /// <inheritdoc />
  public override InterceptionResult<int> SavingChanges(
    DbContextEventData eventData,
    InterceptionResult<int> result)
    => base.SavingChanges(eventData, InterceptionResult<int>.SuppressWithResult(0));

  /// <inheritdoc />
  public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
    DbContextEventData eventData,
    InterceptionResult<int> result,
    CancellationToken cancellationToken = default)
    => await base.SavingChangesAsync(eventData, InterceptionResult<int>.SuppressWithResult(0), cancellationToken);
}
