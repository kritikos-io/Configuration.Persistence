#pragma warning disable SA1402 // File may only contain a single type
namespace Kritikos.Configuration.Persistence.Contracts.Behavioral
{
  /// <summary>
  /// Marker field targeted by interceptors to generate audit trails.
  /// </summary>
  /// <remarks>
  /// Use <see cref="IAuditable{T}"/> when all you need is a simple creation and last audit authority.
  /// </remarks>
  public interface ITraceableAudit
  {
  }

  /// <summary>
  /// Exposes barebones auditing functionality on a multi-user system.
  /// </summary>
  /// <remarks>
  /// Use <see cref="ITraceableAudit"/> when proper audit trails are needed.
  /// </remarks>
  /// <typeparam name="T">Type of auditor identifying field.</typeparam>
  /// <remarks>Fields should be handled by interceptors.</remarks>
  public interface IAuditable<T> : ICreateAuditable<T>, IUpdateAuditable<T>
  {
  }

  /// <summary>
  /// Exposes the person that created this entity.
  /// </summary>
  /// <typeparam name="T">Type of auditor identifying field.</typeparam>
  public interface ICreateAuditable<T>
  {
    T CreatedBy { get; set; }
  }

  /// <summary>
  /// Exposes last person to update this entity.
  /// </summary>
  /// <typeparam name="T">Type of auditor identifying field.</typeparam>
  public interface IUpdateAuditable<T>
  {
    T UpdatedBy { get; set; }
  }
}
