namespace Kritikos.Configuration.Persistence.Interceptors
{
  using System;
  using System.Diagnostics.CodeAnalysis;
  using System.Threading;
  using System.Threading.Tasks;

  using Kritikos.Configuration.Persistence.Abstractions;
  using Kritikos.Configuration.Persistence.Services;

  using Microsoft.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore.Diagnostics;

  /// <summary>
  /// Populates audit values for <see cref="IAuditable{T}"/> entities.
  /// </summary>
  /// <typeparam name="T">Type of audit field.</typeparam>
  public class AuditSaveChangesInterceptor<T> : SaveChangesInterceptor
    where T : IComparable, IComparable<T>, IEquatable<T>
  {
    private readonly IAuditorProvider<T> auditorProvider;

    public AuditSaveChangesInterceptor(IAuditorProvider<T> auditorProvider) =>
      this.auditorProvider = auditorProvider;

    #region Overrides of SaveChangesInterceptor

    /// <inheritdoc />
    [ExcludeFromCodeCoverage] // Handled in async method
    public override InterceptionResult<int> SavingChanges(
      DbContextEventData eventData,
      InterceptionResult<int> result)
    {
      var auditor = auditorProvider.GetAuditor() ?? auditorProvider.GetFallbackAuditor();
      var entries = eventData.Context.ChangeTracker.Entries<IAuditable<T>>()
                    ?? throw new ArgumentNullException(nameof(eventData));

      foreach (var entry in entries)
      {
        if (entry.State == EntityState.Added)
        {
          entry.Entity.CreatedBy = auditor;
        }

        entry.Entity.UpdatedBy = auditor;
      }

      return base.SavingChanges(eventData, result);
    }

    /// <inheritdoc />
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
      DbContextEventData eventData,
      InterceptionResult<int> result,
      CancellationToken cancellationToken = default)
    {
      var auditor = auditorProvider.GetAuditor() ?? auditorProvider.GetFallbackAuditor();
      var entries = eventData.Context.ChangeTracker.Entries<IAuditable<T>>()
                    ?? throw new ArgumentNullException(nameof(eventData));

      foreach (var entry in entries)
      {
        if (entry.State == EntityState.Added)
        {
          entry.Entity.CreatedBy = auditor;
        }

        entry.Entity.UpdatedBy = auditor;
      }

      return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    #endregion Overrides of SaveChangesInterceptor
  }
}
