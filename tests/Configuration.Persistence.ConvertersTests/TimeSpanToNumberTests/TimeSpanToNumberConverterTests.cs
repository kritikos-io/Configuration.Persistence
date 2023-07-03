#pragma warning disable SA1402 // File may only contain a single type
namespace Kritikos.Configuration.Persistence.ConvertersTests.TimeSpanToNumberTests;

using System;

using Kritikos.Configuration.Persistence.Converters.Enums;
using Kritikos.Configuration.Persistence.Converters.Primitive;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using Xunit;

public abstract class TimeSpanToNumberConverterTests
{
  protected static readonly ConverterMappingHints MappingHints = new(unicode: true);

  protected static readonly Func<double, DateInterval, TimeSpan> ToTimeSpan = (value, interval) => interval switch
  {
    DateInterval.Days => TimeSpan.FromDays(value),
    DateInterval.Hours => TimeSpan.FromHours(value),
    DateInterval.Minutes => TimeSpan.FromMinutes(value),
    DateInterval.Seconds => TimeSpan.FromSeconds(value),
    DateInterval.Milliseconds => TimeSpan.FromMilliseconds(value),
    DateInterval.Ticks => TimeSpan.FromTicks(Convert.ToInt64(value)),
    _ => throw new NotImplementedException($"{nameof(interval)} not supported!"),
  };

  protected static readonly Func<TimeSpan, DateInterval, double> FromTimespan = (value, interval) => interval switch
  {
    DateInterval.Days => value.TotalDays,
    DateInterval.Hours => value.TotalHours,
    DateInterval.Minutes => value.TotalMinutes,
    DateInterval.Seconds => value.TotalSeconds,
    DateInterval.Milliseconds => value.TotalMilliseconds,
    DateInterval.Ticks => value.Ticks,
    _ => throw new NotImplementedException($"{nameof(interval)} not supported!"),
  };
}

public abstract class TimeSpanToNumberConverterTests<T> : TimeSpanToNumberConverterTests
  where T : unmanaged, IConvertible, IComparable, IComparable<T>, IEquatable<T>
{
  protected abstract TimeSpanToNumberConverter<T> CreateConverter(DateInterval interval);

  protected void Tester(
    TimeSpan span,
    Func<T, DateInterval, TimeSpan> toTimeSpan,
    Func<TimeSpan, DateInterval, T> fromTimeSpan,
    DateInterval interval)
  {
    ArgumentNullException.ThrowIfNull(fromTimeSpan);
    ArgumentNullException.ThrowIfNull(toTimeSpan);

    var converter = CreateConverter(interval);
    var numberToTimespan = converter.ConvertFromProviderExpression.Compile();
    var timeSpanToNumber = converter.ConvertToProviderExpression.Compile();

    var foo = timeSpanToNumber(span);
    var bar = numberToTimespan(foo);

    Assert.Equal(fromTimeSpan(span, interval), foo);
    Assert.Equal(toTimeSpan(foo, interval), bar);
    Assert.Equal(fromTimeSpan(bar, interval), fromTimeSpan(span, interval));
  }
}
