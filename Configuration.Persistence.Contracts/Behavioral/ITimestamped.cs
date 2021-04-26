namespace Kritikos.Configuration.Persistence.Contracts.Behavioral
{
  using System;

  /// <summary>
  /// Exposes timestamping functionality for barebones persistence auditing.
  /// </summary>
  /// <remarks>Fields should be handled by interceptors.</remarks>
  public interface ITimestamped
  {
    DateTimeOffset CreatedAt { get; set; }

    DateTimeOffset UpdatedAt { get; set; }
  }
}
