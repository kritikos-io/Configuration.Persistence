namespace Kritikos.Configuration.Persistence.ConvertersTests.TimeSpanToNumberTests;

using System;

using Kritikos.Configuration.Persistence.Converters.Enums;
using Kritikos.Configuration.Persistence.Converters.Primitive;

using Xunit;

public class TimeSpanToDoubleTests : TimeSpanToNumberConverterTests<double>
{
  [Fact]
  public void Check_TimeSpan_in_total_days()
  {
    const DateInterval interval = DateInterval.Days;
    Tester(TimeSpan.MaxValue, ToTimeSpan, FromTimespan, interval);
    Tester(TimeSpan.MinValue, ToTimeSpan, FromTimespan, interval);
  }

  [Fact]
  public void Check_TimeSpan_in_total_hours()
  {
    const DateInterval interval = DateInterval.Hours;
    Tester(TimeSpan.MaxValue.Subtract(TimeSpan.FromHours(1)), ToTimeSpan, FromTimespan, interval);
    Tester(TimeSpan.MinValue.Add(TimeSpan.FromHours(1)), ToTimeSpan, FromTimespan, interval);
  }

  [Fact]
  public void Check_TimeSpan_in_total_minutes()
  {
    const DateInterval interval = DateInterval.Minutes;
    Tester(TimeSpan.MaxValue.Subtract(TimeSpan.FromHours(1)), ToTimeSpan, FromTimespan, interval);
    Tester(TimeSpan.MinValue.Add(TimeSpan.FromHours(1)), ToTimeSpan, FromTimespan, interval);
  }

  [Fact]
  public void Check_TimeSpan_in_total_seconds()
  {
    const DateInterval interval = DateInterval.Seconds;
    Tester(TimeSpan.MaxValue.Subtract(TimeSpan.FromHours(1)), ToTimeSpan, FromTimespan, interval);
    Tester(TimeSpan.MinValue.Add(TimeSpan.FromHours(1)), ToTimeSpan, FromTimespan, interval);
  }

  [Fact]
  public void Check_TimeSpan_in_total_milliseconds()
  {
    const DateInterval interval = DateInterval.Milliseconds;
    Tester(TimeSpan.MaxValue.Subtract(TimeSpan.FromHours(1)), ToTimeSpan, FromTimespan, interval);
    Tester(TimeSpan.MinValue.Add(TimeSpan.FromHours(1)), ToTimeSpan, FromTimespan, interval);
  }

  [Fact]
  public void Check_TimeSpan_in_ticks()
  {
    const DateInterval interval = DateInterval.Ticks;
    Tester(TimeSpan.MaxValue.Subtract(TimeSpan.FromHours(1)), ToTimeSpan, FromTimespan, interval);
    Tester(TimeSpan.MinValue.Add(TimeSpan.FromHours(1)), ToTimeSpan, FromTimespan, interval);
  }

  /// <inheritdoc />
  protected override TimeSpanToDoubleConverter CreateConverter(DateInterval interval) => new(interval, MappingHints);
}
