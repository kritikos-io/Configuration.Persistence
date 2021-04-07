namespace Kritikos.Configuration.Persistence.Abstractions
{
  using Kritikos.Configuration.Persistence.Interceptors;

  /// <summary>
  /// Exposes barebones auditing functionality on a multi-user system.
  /// </summary>
  /// <typeparam name="T">Type of auditor identifying field.</typeparam>
  /// <remarks>
  /// Fields are set automatically via <see cref="AuditSaveChangesInterceptor{T}"/>.
  /// </remarks>
  public interface IAuditable<T>
  {
    T CreatedBy { get; set; }

    T UpdatedBy { get; set; }
  }
}
