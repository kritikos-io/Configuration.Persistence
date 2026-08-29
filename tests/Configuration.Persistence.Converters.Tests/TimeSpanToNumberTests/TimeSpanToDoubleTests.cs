namespace Kritikos.Configuration.Persistence.Converters.Tests.TimeSpanToNumberTests;

using System;

using Kritikos.Configuration.Persistence.Converters.Enums;
using Kritikos.Configuration.Persistence.Converters.Primitive;

public class TimeSpanToDoubleTests : TimeSpanToNumberConverterTests<double>
{
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
  [Arguments(DateInterval.Days)]
  [Arguments(DateInterval.Hours)]
  [Arguments(DateInterval.Minutes)]
  [Arguments(DateInterval.Seconds)]
  [Arguments(DateInterval.Milliseconds)]
  [Arguments(DateInterval.Ticks)]
  public async Task ConvertFromProvider_ValueAboveTimeSpanMaximum_ThrowsArgumentOutOfRangeException(
    DateInterval interval)
  {
    var fromProvider = CreateConverter(interval).ConvertFromProviderExpression.Compile();

    await Assert.That(() => fromProvider(double.MaxValue))
      .Throws<ArgumentOutOfRangeException>();
  }

  [Test]
  [Arguments(DateInterval.Days)]
  [Arguments(DateInterval.Hours)]
  [Arguments(DateInterval.Minutes)]
  [Arguments(DateInterval.Seconds)]
  [Arguments(DateInterval.Milliseconds)]
  [Arguments(DateInterval.Ticks)]
  public async Task ConvertFromProvider_ValueBelowTimeSpanMinimum_ThrowsArgumentOutOfRangeException(
    DateInterval interval)
  {
    var fromProvider = CreateConverter(interval).ConvertFromProviderExpression.Compile();

    await Assert.That(() => fromProvider(double.MinValue))
      .Throws<ArgumentOutOfRangeException>();
  }

  [Test]
  [Arguments(DateInterval.Days)]
  [Arguments(DateInterval.Hours)]
  [Arguments(DateInterval.Minutes)]
  [Arguments(DateInterval.Seconds)]
  [Arguments(DateInterval.Milliseconds)]
  [Arguments(DateInterval.Ticks)]
  public async Task ConvertFromProvider_NotANumber_ThrowsArgumentOutOfRangeException(DateInterval interval)
  {
    var fromProvider = CreateConverter(interval).ConvertFromProviderExpression.Compile();

    await Assert.That(() => fromProvider(double.NaN))
      .Throws<ArgumentOutOfRangeException>();
  }

  [Test]
  public async Task ConvertFromProvider_TicksBeyondInt64ButRepresentableAsDouble_ThrowsArgumentOutOfRangeException()
  {
    var fromProvider = CreateConverter(DateInterval.Ticks).ConvertFromProviderExpression.Compile();

    // 2^63 rounds to the same double as long.MaxValue, so the bounds check alone cannot reject it.
    await Assert.That(() => fromProvider(9223372036854775808d))
      .Throws<ArgumentOutOfRangeException>();
  }

  [Test]
  public async Task ConvertToProvider_UnsupportedInterval_ThrowsInvalidOperationException()
  {
    var toProvider = CreateConverter((DateInterval)byte.MaxValue).ConvertToProviderExpression.Compile();

    await Assert.That(() => toProvider(TimeSpan.Zero))
      .Throws<InvalidOperationException>();
  }

  [Test]
  public async Task ConvertFromProvider_UnsupportedInterval_ThrowsInvalidOperationException()
  {
    var fromProvider = CreateConverter((DateInterval)byte.MaxValue).ConvertFromProviderExpression.Compile();

    await Assert.That(() => fromProvider(0d))
      .Throws<InvalidOperationException>();
  }

  /// <inheritdoc />
  protected override TimeSpanToDoubleConverter CreateConverter(DateInterval interval) => new(interval, MappingHints);
}
