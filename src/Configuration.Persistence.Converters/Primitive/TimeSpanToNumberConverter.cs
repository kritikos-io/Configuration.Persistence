#pragma warning disable SA1402 // File may only contain a single type
namespace Kritikos.Configuration.Persistence.Converters.Primitive;

using System;
using System.Globalization;

using Kritikos.Configuration.Persistence.Converters.Enums;

using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

/// <summary>
/// Converts <seealso cref="TimeSpan"/> to and from numeric types with specified <seealso cref="DateInterval"/>.
/// </summary>
/// <remarks>
/// Conversion to <typeparamref name="T"/> throws an <see cref="OverflowException"/> when the requested <seealso cref="DateInterval"/> yields a value outside the range of <typeparamref name="T"/>, and rounds to the nearest representable value when <typeparamref name="T"/> is integral, so a round trip is lossy below the chosen interval.
/// Conversion from <typeparamref name="T"/> throws an <see cref="ArgumentOutOfRangeException"/> for stored values outside the range of <seealso cref="TimeSpan"/>, including <see cref="double.NaN"/>.
/// </remarks>
/// <typeparam name="T">The numeric value type to convert to and from.</typeparam>
/// <param name="interval">The interval used in the numeric representation.</param>
/// <param name="mappingHints">
/// Hints that can be used by the <see cref="ITypeMappingSource" /> to create data types with appropriate
/// facets for the converted data.
/// </param>
public class TimeSpanToNumberConverter<T>(DateInterval interval, ConverterMappingHints? mappingHints = null)
  : ValueConverter<TimeSpan, T>(
    v => NumberFromTimeSpan(interval, v),
    v => TimeSpanToNumber(interval, v),
    mappingHints)
  where T : unmanaged, IConvertible, IComparable, IComparable<T>, IEquatable<T>
{
  private static T NumberFromTimeSpan(DateInterval interval, TimeSpan span)
    => interval switch
    {
      DateInterval.Days => ToNumber(span.TotalDays),
      DateInterval.Hours => ToNumber(span.TotalHours),
      DateInterval.Minutes => ToNumber(span.TotalMinutes),
      DateInterval.Seconds => ToNumber(span.TotalSeconds),
      DateInterval.Milliseconds => ToNumber(span.TotalMilliseconds),
      DateInterval.Ticks => ToNumber(span.Ticks),
      _ => throw new InvalidOperationException($"{nameof(interval)} is not supported."),
    };

  // Each arm converts on its own, so ticks are never unified into the double the other intervals produce.
  private static T ToNumber<TValue>(TValue value)
    where TValue : struct
    => (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);

  private static TimeSpan TimeSpanToNumber(DateInterval interval, T val)
  {
    var value = Convert.ToDouble(val, CultureInfo.InvariantCulture);

    return interval switch
    {
      DateInterval.Days => InRange(value, TimeSpan.MinValue.TotalDays, TimeSpan.MaxValue.TotalDays)
        ? TimeSpan.FromDays(value)
        : throw OutOfRange(interval, value, TimeSpan.MinValue.TotalDays, TimeSpan.MaxValue.TotalDays),
      DateInterval.Hours => InRange(value, TimeSpan.MinValue.TotalHours, TimeSpan.MaxValue.TotalHours)
        ? TimeSpan.FromHours(value)
        : throw OutOfRange(interval, value, TimeSpan.MinValue.TotalHours, TimeSpan.MaxValue.TotalHours),
      DateInterval.Minutes => InRange(value, TimeSpan.MinValue.TotalMinutes, TimeSpan.MaxValue.TotalMinutes)
        ? TimeSpan.FromMinutes(value)
        : throw OutOfRange(interval, value, TimeSpan.MinValue.TotalMinutes, TimeSpan.MaxValue.TotalMinutes),
      DateInterval.Seconds => InRange(value, TimeSpan.MinValue.TotalSeconds, TimeSpan.MaxValue.TotalSeconds)
        ? TimeSpan.FromSeconds(value)
        : throw OutOfRange(interval, value, TimeSpan.MinValue.TotalSeconds, TimeSpan.MaxValue.TotalSeconds),
      DateInterval.Milliseconds => InRange(value, TimeSpan.MinValue.TotalMilliseconds, TimeSpan.MaxValue.TotalMilliseconds)
        ? TimeSpan.FromMilliseconds(value)
        : throw OutOfRange(interval, value, TimeSpan.MinValue.TotalMilliseconds, TimeSpan.MaxValue.TotalMilliseconds),

      // The bounds are compared as double but the conversion reads the original value, so an integral T keeps every tick.
      DateInterval.Ticks => TicksToTimeSpan(val, value),
      _ => throw new InvalidOperationException($"{nameof(interval)} is not supported."),
    };
  }

  private static TimeSpan TicksToTimeSpan(T val, double value)
  {
    // long.MaxValue is not representable as a double, so a tick count at the bound compares equal to it rather than below.
    if (value is < long.MinValue or > long.MaxValue)
    {
      throw OutOfRange(DateInterval.Ticks, value, long.MinValue, long.MaxValue);
    }

    try
    {
      return TimeSpan.FromTicks(Convert.ToInt64(val, CultureInfo.InvariantCulture));
    }
    catch (OverflowException)
    {
      throw OutOfRange(DateInterval.Ticks, value, long.MinValue, long.MaxValue);
    }
  }

  private static bool InRange(double value, double minimum, double maximum)
    => value >= minimum && value <= maximum;

  private static ArgumentOutOfRangeException OutOfRange(DateInterval interval, double value, double minimum, double maximum)
    => new(
      "val",
      value,
      $"Supported amount of {interval} ranges from {minimum} to {maximum}");
}

/// <summary>
/// Converts <seealso cref="TimeSpan"/> to and from <see cref="double"/> using a specified <seealso cref="DateInterval"/>.
/// </summary>
/// <param name="interval">The interval used in the numeric double representation.</param>
/// <param name="mappingHints">
/// Hints that can be used by the <see cref="ITypeMappingSource" /> to create data types with appropriate
/// facets for the converted data.
/// </param>
public class TimeSpanToDoubleConverter(DateInterval interval, ConverterMappingHints? mappingHints = null)
  : TimeSpanToNumberConverter<double>(interval, mappingHints);

/// <summary>
/// Converts <seealso cref="TimeSpan"/> to and from <see cref="long"/> using a specified <seealso cref="DateInterval"/>.
/// </summary>
/// <param name="interval">The interval used in the numeric long representation.</param>
/// <param name="mappingHints">
/// Hints that can be used by the <see cref="ITypeMappingSource" /> to create data types with appropriate
/// facets for the converted data.
/// </param>
public class TimeSpanToLongConverter(DateInterval interval, ConverterMappingHints? mappingHints = null)
  : TimeSpanToNumberConverter<long>(interval, mappingHints);

/// <summary>
/// Converts <seealso cref="TimeSpan"/> to and from <see cref="int"/> using a specified <seealso cref="DateInterval"/>.
/// </summary>
/// <remarks>Care required when using this converter for <seealso cref="DateInterval.Minutes"/> and smaller denominations, since conversion to <see cref="int"/> throws an <see cref="OverflowException"/> once the interval count exceeds <see cref="int.MaxValue"/>.</remarks>
/// <param name="interval">The interval used in the numeric int representation.</param>
/// <param name="mappingHints">
/// Hints that can be used by the <see cref="ITypeMappingSource" /> to create data types with appropriate
/// facets for the converted data.
/// </param>
public class TimeSpanToIntConverter(DateInterval interval, ConverterMappingHints? mappingHints = null)
  : TimeSpanToNumberConverter<int>(interval, mappingHints);
