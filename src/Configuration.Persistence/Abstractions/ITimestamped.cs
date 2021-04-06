namespace Kritikos.Configuration.Persistence.Abstractions
{
  using System;

  using Kritikos.Configuration.Persistence.Interceptors;

  /// <summary>
  /// Exposes timestamping functionality for barebones persistence auditing.
  /// </summary>
  /// <remarks>
  /// Fields are set automatically via <see cref="AuditSaveChangesInterceptor{T}"/>.
  /// </remarks>
  public interface ITimestamped
  {
    DateTimeOffset CreatedAt { get; set; }

    DateTimeOffset UpdatedAt { get; set; }
  }
}
