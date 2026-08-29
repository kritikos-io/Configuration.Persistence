namespace Kritikos.Configuration.Persistence.TestKit;

using System;

public sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
{
  public DateTimeOffset UtcNow { get; set; } = utcNow;

  public override DateTimeOffset GetUtcNow() => UtcNow;
}
