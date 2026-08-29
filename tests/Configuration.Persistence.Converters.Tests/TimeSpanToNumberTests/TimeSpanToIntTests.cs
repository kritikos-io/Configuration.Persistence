namespace Kritikos.Configuration.Persistence.Converters.Tests.TimeSpanToNumberTests;

using System;

using Kritikos.Configuration.Persistence.Converters.Enums;
using Kritikos.Configuration.Persistence.Converters.Primitive;

public class TimeSpanToIntTests : TimeSpanToNumberConverterTests<int>
{
  private static new readonly Func<TimeSpan, DateInterval, int> FromTimeSpan = (span, interval) =>
    Convert.ToInt32(TimeSpanToNumberConverterTests.FromTimeSpan(span, interval));

  private static new readonly Func<int, DateInterval, TimeSpan> ToTimeSpan = (value, interval) =>
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
  [Arguments(DateInterval.Minutes)]
  [Arguments(DateInterval.Seconds)]
  [Arguments(DateInterval.Milliseconds)]
  [Arguments(DateInterval.Ticks)]
  public async Task ConvertToProvider_IntervalCountAboveIntMaximum_ThrowsOverflowException(DateInterval interval)
  {
    var toProvider = CreateConverter(interval).ConvertToProviderExpression.Compile();

    await Assert.That(() => toProvider(TimeSpan.MaxValue))
      .Throws<OverflowException>();
  }

  /// <inheritdoc />
  protected override TimeSpanToIntConverter CreateConverter(DateInterval interval) => new(interval, MappingHints);
}
