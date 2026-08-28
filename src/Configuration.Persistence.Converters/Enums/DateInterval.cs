namespace Kritikos.Configuration.Persistence.Converters.Enums;

/// <summary>
/// The unit a <see cref="System.TimeSpan"/> is persisted in.
/// </summary>
public enum DateInterval
{
  /// <summary>Whole days.</summary>
  Days,

  /// <summary>Whole hours.</summary>
  Hours,

  /// <summary>Whole minutes.</summary>
  Minutes,

  /// <summary>Whole seconds.</summary>
  Seconds,

  /// <summary>Whole milliseconds.</summary>
  Milliseconds,

  /// <summary>Raw <see cref="System.TimeSpan.Ticks"/>, the highest available resolution.</summary>
  Ticks,
}
