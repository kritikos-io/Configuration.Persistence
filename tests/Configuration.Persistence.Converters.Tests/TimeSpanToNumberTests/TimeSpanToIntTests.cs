namespace Kritikos.Configuration.Persistence.Converters.Tests.TimeSpanToNumberTests;

using System;

using Kritikos.Configuration.Persistence.Converters.Enums;
using Kritikos.Configuration.Persistence.Converters.Primitive;

public class TimeSpanToIntTests : TimeSpanToNumberConverterTests<int>
{
  private static new readonly Func<TimeSpan, DateInterval, int> FromTimespan = (span, interval) =>
    Convert.ToInt32(TimeSpanToNumberConverterTests.FromTimespan(span, interval));

  private static new readonly Func<int, DateInterval, TimeSpan> ToTimeSpan = (value, interval) =>
    TimeSpanToNumberConverterTests.ToTimeSpan(Convert.ToDouble(value), interval);

  [Test]
  public async Task Convert_DaysIntervalAtTimeSpanBounds_RoundTripsExactly()
  {
    const DateInterval interval = DateInterval.Days;
    await Tester(TimeSpan.MaxValue, ToTimeSpan, FromTimespan, interval);
    await Tester(TimeSpan.MinValue, ToTimeSpan, FromTimespan, interval);
  }

  [Test]
  public async Task Convert_HoursIntervalNearTimeSpanBounds_RoundTripsExactly()
  {
    const DateInterval interval = DateInterval.Hours;
    await Tester(TimeSpan.MaxValue.Subtract(TimeSpan.FromHours(1)), ToTimeSpan, FromTimespan, interval);
    await Tester(TimeSpan.MinValue.Add(TimeSpan.FromHours(1)), ToTimeSpan, FromTimespan, interval);
  }

  /// <inheritdoc />
  protected override TimeSpanToIntConverter CreateConverter(DateInterval interval) => new(interval, MappingHints);
}
