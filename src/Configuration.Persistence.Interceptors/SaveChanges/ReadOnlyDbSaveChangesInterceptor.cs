namespace Kritikos.Configuration.Persistence.Interceptors.SaveChanges;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

/// <summary>
/// Makes a <see cref="DbContext"/> readonly by intercepting and preventing any attempt to save changes.
/// </summary>
/// <remarks>
/// Adding this class to the list of interceptors for an instance of a <see cref="DbContext"/> is final, consider using <see cref="QueryTrackingBehavior"/> for a runtime mutable behavior.
/// </remarks>
public class ReadOnlyDbSaveChangesInterceptor : SaveChangesInterceptor
{
  /// <inheritdoc />
  [ExcludeFromCodeCoverage] // Handled in async method
  public override InterceptionResult<int> SavingChanges(
    DbContextEventData eventData,
    InterceptionResult<int> result)
  {
    var suppresedResult = InterceptionResult<int>.SuppressWithResult(0);

    return base.SavingChanges(eventData, suppresedResult);
  }

  /// <inheritdoc />
  public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
    DbContextEventData eventData,
    InterceptionResult<int> result,
    CancellationToken cancellationToken = default)
  {
    var suppresedResult = InterceptionResult<int>.SuppressWithResult(0);

    return await base.SavingChangesAsync(eventData, suppresedResult, cancellationToken);
  }
}
