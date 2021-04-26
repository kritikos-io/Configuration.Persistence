namespace Kritikos.Configuration.Persistence.Contracts.Behavioral
{
  /// <summary>
  /// Exposes barebones auditing functionality on a multi-user system.
  /// </summary>
  /// <typeparam name="T">Type of auditor identifying field.</typeparam>
  /// <remarks>Fields should be handled by interceptors.</remarks>
  public interface IAuditable<T>
  {
    T CreatedBy { get; set; }

    T UpdatedBy { get; set; }
  }
}
