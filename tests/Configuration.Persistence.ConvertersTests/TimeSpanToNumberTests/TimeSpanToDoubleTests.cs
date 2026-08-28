namespace Kritikos.Configuration.Persistence.ConvertersTests.TimeSpanToNumberTests;

using System;

using Kritikos.Configuration.Persistence.Converters.Enums;
using Kritikos.Configuration.Persistence.Converters.Primitive;

public class TimeSpanToDoubleTests : TimeSpanToNumberConverterTests<double>
{
  [Test]
  public async Task Check_TimeSpan_in_total_days()
  {
    const DateInterval interval = DateInterval.Days;
    await Tester(TimeSpan.MaxValue, ToTimeSpan, FromTimespan, interval);
    await Tester(TimeSpan.MinValue, ToTimeSpan, FromTimespan, interval);
  }

  [Test]
  public async Task Check_TimeSpan_in_total_hours()
  {
    const DateInterval interval = DateInterval.Hours;
    await Tester(TimeSpan.MaxValue.Subtract(TimeSpan.FromHours(1)), ToTimeSpan, FromTimespan, interval);
    await Tester(TimeSpan.MinValue.Add(TimeSpan.FromHours(1)), ToTimeSpan, FromTimespan, interval);
  }

  [Test]
  public async Task Check_TimeSpan_in_total_minutes()
  {
    const DateInterval interval = DateInterval.Minutes;
    await Tester(TimeSpan.MaxValue.Subtract(TimeSpan.FromHours(1)), ToTimeSpan, FromTimespan, interval);
    await Tester(TimeSpan.MinValue.Add(TimeSpan.FromHours(1)), ToTimeSpan, FromTimespan, interval);
  }

  [Test]
  public async Task Check_TimeSpan_in_total_seconds()
  {
    const DateInterval interval = DateInterval.Seconds;
    await Tester(TimeSpan.MaxValue.Subtract(TimeSpan.FromHours(1)), ToTimeSpan, FromTimespan, interval);
    await Tester(TimeSpan.MinValue.Add(TimeSpan.FromHours(1)), ToTimeSpan, FromTimespan, interval);
  }

  [Test]
  public async Task Check_TimeSpan_in_total_milliseconds()
  {
    const DateInterval interval = DateInterval.Milliseconds;
    await Tester(TimeSpan.MaxValue.Subtract(TimeSpan.FromHours(1)), ToTimeSpan, FromTimespan, interval);
    await Tester(TimeSpan.MinValue.Add(TimeSpan.FromHours(1)), ToTimeSpan, FromTimespan, interval);
  }

  [Test]
  public async Task Check_TimeSpan_in_ticks()
  {
    const DateInterval interval = DateInterval.Ticks;
    await Tester(TimeSpan.MaxValue.Subtract(TimeSpan.FromHours(1)), ToTimeSpan, FromTimespan, interval);
    await Tester(TimeSpan.MinValue.Add(TimeSpan.FromHours(1)), ToTimeSpan, FromTimespan, interval);
  }

  /// <inheritdoc />
  protected override TimeSpanToDoubleConverter CreateConverter(DateInterval interval) => new(interval, MappingHints);
}
