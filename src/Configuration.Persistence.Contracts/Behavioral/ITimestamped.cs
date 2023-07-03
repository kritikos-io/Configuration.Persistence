#pragma warning disable SA1402 // File may only contain a single type
namespace Kritikos.Configuration.Persistence.Contracts.Behavioral;

using System;

/// <summary>
/// Exposes creation and last update time in UTC.
/// </summary>
/// <remarks>Fields should be handled by interceptors.</remarks>
public interface ITimestamped : ICreateTimestamped, IUpdateTimestamped
{
}

/// <summary>
/// Exposes creation time in UTC for this entity.
/// </summary>
public interface ICreateTimestamped
{
  DateTime CreatedAt { get; set; }
}

/// <summary>
/// Exposes last update time in UTC for this entity.
/// </summary>
public interface IUpdateTimestamped
{
  DateTime UpdatedAt { get; set; }
}
