#pragma warning disable SA1402 // File may only contain a single type
namespace Kritikos.Configuration.Persistence.Converters
{
	using System;

	using Kritikos.Configuration.Persistence.Enums;

	using Microsoft.EntityFrameworkCore.Storage;
	using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

	/// <summary>
	/// Converts <seealso cref="TimeSpan"/> to and from numeric types with specified <seealso cref="DateInterval"/>.
	/// </summary>
	/// <typeparam name="T">The numeric value to convert to and from.</typeparam>
	/// <exception cref="OverflowException"><seealso cref="DateInterval"/> requested exceeds the max value of <typeparamref name="T"/>.</exception>
	public class TimeSpanToNumberConverter<T> : ValueConverter<TimeSpan, T>
		where T : unmanaged, IConvertible, IComparable, IComparable<T>, IEquatable<T>
	{
		/// <summary>
		/// Initializes a new instance of the <seealso cref="TimeSpanToNumberConverter{T}"/> class.
		/// </summary>
		/// <param name="interval">The interval used in the numeric representation.</param>
		/// <param name="mappingHints">
		/// Hints that can be used by the <see cref="ITypeMappingSource" /> to create data types with appropriate
		/// facets for the converted data.
		/// </param>
		public TimeSpanToNumberConverter(DateInterval interval, ConverterMappingHints? mappingHints = null)
			: base(
				v => NumberFromTimeSpan(interval, v),
				v => TimeSpanToNumber(interval, v),
				mappingHints)
		{
		}

		private static T NumberFromTimeSpan(DateInterval interval, TimeSpan span)
		{
			var value = interval switch
			{
				DateInterval.Days => span.TotalDays,
				DateInterval.Hours => span.TotalHours,
				DateInterval.Minutes => span.TotalMinutes,
				DateInterval.Seconds => span.TotalSeconds,
				DateInterval.Milliseconds => span.TotalMilliseconds,
				DateInterval.Ticks => Convert.ToDouble(span.Ticks),
				_ => throw new NotImplementedException($"{nameof(interval)} is not supported."),
			};

			var result = (T)Convert.ChangeType(value, typeof(T));
			return result;
		}

		private static TimeSpan TimeSpanToNumber(DateInterval interval, T val)
		{
			var value = (double)Convert.ChangeType(val, typeof(double));

			return interval switch
			{
				DateInterval.Days => value <= TimeSpan.MaxValue.TotalDays
					? TimeSpan.FromDays(value)
					: throw new ArgumentOutOfRangeException(
						nameof(val),
						value,
						$"Maximum amount of {interval} supported is {TimeSpan.MaxValue.TotalDays}"),
				DateInterval.Hours => value <= TimeSpan.MaxValue.TotalHours
					? TimeSpan.FromHours(value)
					: throw new ArgumentOutOfRangeException(
						nameof(val),
						value,
						$"Maximum amount of {interval} supported is {TimeSpan.MaxValue.TotalHours}"),
				DateInterval.Minutes => value <= TimeSpan.MaxValue.TotalMinutes
					? TimeSpan.FromMinutes(value)
					: throw new ArgumentOutOfRangeException(
						nameof(val),
						value,
						$"Maximum amount of {interval} supported is {TimeSpan.MaxValue.TotalMinutes}"),
				DateInterval.Seconds => value <= TimeSpan.MaxValue.TotalSeconds
					? TimeSpan.FromSeconds(value)
					: throw new ArgumentOutOfRangeException(
						nameof(val),
						value,
						$"Maximum amount of {interval} supported is {TimeSpan.MaxValue.TotalSeconds}"),
				DateInterval.Milliseconds => value <= TimeSpan.MaxValue.TotalMilliseconds
					? TimeSpan.FromMilliseconds(value)
					: throw new ArgumentOutOfRangeException(
						nameof(val),
						value,
						$"Maximum amount of {interval} supported is {TimeSpan.MaxValue.TotalMilliseconds}"),
				DateInterval.Ticks => value <= TimeSpan.MaxValue.Ticks
					? TimeSpan.FromTicks(Convert.ToInt64(value))
					: throw new ArgumentOutOfRangeException(
						nameof(val),
						value,
						$"Maximum amount of {interval} supported is {TimeSpan.MaxValue.Ticks}"),
				_ => throw new NotImplementedException($"{nameof(interval)} is not supported."),
			};
		}
	}

	/// <summary>
	/// Converts <seealso cref="TimeSpan"/> to and from <see cref="double"/> using a specified <seealso cref="DateInterval"/>.
	/// </summary>
	public class TimeSpanToDoubleConverter : TimeSpanToNumberConverter<double>
	{
		/// <summary>
		/// Initializes a new instance of the <seealso cref="TimeSpanToDoubleConverter"/> class.
		/// </summary>
		/// <param name="interval">The interval used in the numeric double representation.</param>
		/// <param name="mappingHints">
		/// Hints that can be used by the <see cref="ITypeMappingSource" /> to create data types with appropriate
		/// facets for the converted data.
		/// </param>
		public TimeSpanToDoubleConverter(DateInterval interval, ConverterMappingHints? mappingHints = null)
			: base(interval, mappingHints)
		{
		}
	}

	/// <summary>
	/// Converts <seealso cref="TimeSpan"/> to and from <see cref="long"/> using a specified <seealso cref="DateInterval"/>.
	/// </summary>
	public class TimeSpanToLongConverter : TimeSpanToNumberConverter<long>
	{
		/// <summary>
		/// Initializes a new instance of the <seealso cref="TimeSpanToLongConverter"/> class.
		/// </summary>
		/// <param name="interval">The interval used in the numeric double representation.</param>
		/// <param name="mappingHints">
		/// Hints that can be used by the <see cref="ITypeMappingSource" /> to create data types with appropriate
		/// facets for the converted data.
		/// </param>
		public TimeSpanToLongConverter(DateInterval interval, ConverterMappingHints? mappingHints = null)
			: base(interval, mappingHints)
		{
		}
	}

	/// <summary>
	/// Converts <seealso cref="TimeSpan"/> to and from <see cref="int"/> using a specified <seealso cref="DateInterval"/>.
	/// </summary>
	/// <remarks>Care required when using this converter for <seealso cref="DateInterval.Minutes"/> and smaller denominations.</remarks>
	/// <exception cref="OverflowException"><seealso cref="DateInterval"/> requested exceeds the max value of <see cref="int"/>.</exception>
	public class TimeSpanToIntConverter : TimeSpanToNumberConverter<int>
	{
		/// <summary>
		/// Initializes a new instance of the <seealso cref="TimeSpanToIntConverter"/> class.
		/// </summary>
		/// <param name="interval">The interval used in the numeric double representation.</param>
		/// <param name="mappingHints">
		/// Hints that can be used by the <see cref="ITypeMappingSource" /> to create data types with appropriate
		/// facets for the converted data.
		/// </param>
		public TimeSpanToIntConverter(DateInterval interval, ConverterMappingHints? mappingHints = null)
			: base(interval, mappingHints)
		{
		}
	}
}
