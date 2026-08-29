namespace Kritikos.Configuration.Persistence.Converters.Tests.TimeSpanToNumberTests;

using System;

using Kritikos.Configuration.Persistence.Converters.Enums;
using Kritikos.Configuration.Persistence.Converters.Primitive;

public class TimeSpanToLongTests : TimeSpanToNumberConverterTests<long>
{
  // Ticks are compared as Int64 so the expectation does not lose the precision the converter preserves.
  private static new readonly Func<TimeSpan, DateInterval, long> FromTimeSpan = (span, interval) =>
    interval == DateInterval.Ticks
      ? span.Ticks
      : Convert.ToInt64(TimeSpanToNumberConverterTests.FromTimeSpan(span, interval));

  private static new readonly Func<long, DateInterval, TimeSpan> ToTimeSpan = (value, interval) =>
    interval == DateInterval.Ticks
      ? TimeSpan.FromTicks(value)
      : TimeSpanToNumberConverterTests.ToTimeSpan(Convert.ToDouble(value), interval);

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

  [Test]
  [Arguments(long.MaxValue)]
  [Arguments(long.MinValue)]
  public async Task Convert_TicksIntervalAtTimeSpanBounds_RoundTripsExactly(long ticks)
  {
    var converter = CreateConverter(DateInterval.Ticks);
    var toProvider = converter.ConvertToProviderExpression.Compile();
    var fromProvider = converter.ConvertFromProviderExpression.Compile();

    var stored = toProvider(TimeSpan.FromTicks(ticks));

    await Assert.That(stored).IsEqualTo(ticks);
    await Assert.That(fromProvider(stored).Ticks).IsEqualTo(ticks);
  }

  [Test]
  [Arguments(500, 0L)]
  [Arguments(1500, 2L)]
  [Arguments(2500, 2L)]
  [Arguments(3500, 4L)]
  [Arguments(900, 1L)]
  [Arguments(-1500, -2L)]
  public async Task ConvertToProvider_FractionOfTheInterval_RoundsToEven(int milliseconds, long expected)
  {
    var converter = CreateConverter(DateInterval.Seconds);

    var stored = converter.ConvertToProvider(TimeSpan.FromMilliseconds(milliseconds));

    await Assert.That(stored).IsEqualTo(expected);
  }

  [Test]
  public async Task Convert_HalfOfTheIntervalRoundingUp_ReturnsALongerSpanThanItWasGiven()
  {
    var converter = CreateConverter(DateInterval.Seconds);
    var original = TimeSpan.FromMilliseconds(1500);

    var restored = (TimeSpan)converter.ConvertFromProvider(converter.ConvertToProvider(original))!;

    await Assert.That(restored).IsEqualTo(TimeSpan.FromSeconds(2));
    await Assert.That(restored).IsGreaterThan(original);
  }

  /// <inheritdoc />
  protected override TimeSpanToLongConverter CreateConverter(DateInterval interval) => new(interval, MappingHints);
}
