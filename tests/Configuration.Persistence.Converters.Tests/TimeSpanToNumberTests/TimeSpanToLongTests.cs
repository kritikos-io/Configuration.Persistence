namespace Kritikos.Configuration.Persistence.Converters.Tests.TimeSpanToNumberTests;

using System;

using Kritikos.Configuration.Persistence.Converters.Enums;
using Kritikos.Configuration.Persistence.Converters.Primitive;

public class TimeSpanToLongTests : TimeSpanToNumberConverterTests<long>
{
  private static new readonly Func<TimeSpan, DateInterval, long> FromTimeSpan = (span, interval) =>
    Convert.ToInt64(TimeSpanToNumberConverterTests.FromTimeSpan(span, interval));

  private static new readonly Func<long, DateInterval, TimeSpan> ToTimeSpan = (value, interval) =>
    TimeSpanToNumberConverterTests.ToTimeSpan(Convert.ToDouble(value), interval);

  [Test]
  public async Task Convert_DaysIntervalAtTimeSpanBounds_RoundTripsExactly()
  {
    const DateInterval interval = DateInterval.Days;
    await Tester(TimeSpan.MaxValue, ToTimeSpan, FromTimeSpan, interval);
    await Tester(TimeSpan.MinValue, ToTimeSpan, FromTimeSpan, interval);
  }

  [Test]
  public async Task Convert_HoursIntervalNearTimeSpanBounds_RoundTripsExactly()
  {
    const DateInterval interval = DateInterval.Hours;
    await Tester(TimeSpan.MaxValue.Subtract(TimeSpan.FromHours(1)), ToTimeSpan, FromTimeSpan, interval);
    await Tester(TimeSpan.MinValue.Add(TimeSpan.FromHours(1)), ToTimeSpan, FromTimeSpan, interval);
  }

  [Test]
  public async Task Convert_MinutesIntervalNearTimeSpanBounds_RoundTripsExactly()
  {
    const DateInterval interval = DateInterval.Minutes;
    await Tester(TimeSpan.MaxValue.Subtract(TimeSpan.FromHours(1)), ToTimeSpan, FromTimeSpan, interval);
    await Tester(TimeSpan.MinValue.Add(TimeSpan.FromHours(1)), ToTimeSpan, FromTimeSpan, interval);
  }

  [Test]
  public async Task Convert_SecondsIntervalNearTimeSpanBounds_RoundTripsExactly()
  {
    const DateInterval interval = DateInterval.Seconds;
    await Tester(TimeSpan.MaxValue.Subtract(TimeSpan.FromHours(1)), ToTimeSpan, FromTimeSpan, interval);
    await Tester(TimeSpan.MinValue.Add(TimeSpan.FromHours(1)), ToTimeSpan, FromTimeSpan, interval);
  }

  [Test]
  public async Task Convert_MillisecondsIntervalNearTimeSpanBounds_RoundTripsExactly()
  {
    const DateInterval interval = DateInterval.Milliseconds;
    await Tester(TimeSpan.MaxValue.Subtract(TimeSpan.FromHours(1)), ToTimeSpan, FromTimeSpan, interval);
    await Tester(TimeSpan.MinValue.Add(TimeSpan.FromHours(1)), ToTimeSpan, FromTimeSpan, interval);
  }

  [Test]
  public async Task Convert_TicksIntervalNearTimeSpanBounds_RoundTripsExactly()
  {
    const DateInterval interval = DateInterval.Ticks;
    await Tester(TimeSpan.MaxValue.Subtract(TimeSpan.FromHours(1)), ToTimeSpan, FromTimeSpan, interval);
    await Tester(TimeSpan.MinValue.Add(TimeSpan.FromHours(1)), ToTimeSpan, FromTimeSpan, interval);
  }

  /// <inheritdoc />
  protected override TimeSpanToLongConverter CreateConverter(DateInterval interval) => new(interval, MappingHints);
}
