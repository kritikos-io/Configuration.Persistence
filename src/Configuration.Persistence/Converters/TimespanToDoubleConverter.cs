namespace Kritikos.Configuration.Persistence.Converters
{
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;

	using Kritikos.Configuration.Persistence.Enums;

	using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

	/// <summary>
	/// Converts <see cref="TimeSpan"/> to and from <see cref="double"/>.
	/// </summary>
	public class TimespanToDoubleConverter : ValueConverter<TimeSpan, double>
	{
		private static readonly Dictionary<DateInterval, (Expression<Func<double, TimeSpan>> from,
			Expression<Func<TimeSpan, double>> to)> TimeSpanExpressions
			= new Dictionary<DateInterval, (Expression<Func<double, TimeSpan>> from, Expression<Func<TimeSpan, double>>
				to)>
			{
				{ DateInterval.Days, (d => TimeSpan.FromDays(d), span => span.TotalDays) },
				{ DateInterval.Hours, (d => TimeSpan.FromHours(d), span => span.TotalHours) },
				{ DateInterval.Minutes, (d => TimeSpan.FromMinutes(d), span => span.TotalMinutes) },
				{ DateInterval.Seconds, (d => TimeSpan.FromSeconds(d), span => span.TotalSeconds) },
				{ DateInterval.Milliseconds, (d => TimeSpan.FromMilliseconds(d), span => span.TotalMilliseconds) },
			};

		/// <summary>
		/// Creates a new instance of this converter.
		/// </summary>
		/// <param name="interval"><see cref="DateInterval"/> that the <see cref="double"/> value represents.</param>
		/// <param name="mappingHints">Specifies hints used by the type mapper when using a <see cref="ValueConverter"/>.</param>
		public TimespanToDoubleConverter(
			DateInterval interval,
			ConverterMappingHints? mappingHints = null)
			: base(TimeSpanExpressions[interval].to, TimeSpanExpressions[interval].from, mappingHints)
		{
		}
	}

#pragma warning disable SA1402 // File may only contain a single type
	/// <summary>
	/// Converts <see cref="TimeSpan"/> to and from <see cref="long"/>.
	/// </summary>
	public class TimespanToLongConverter : ValueConverter<TimeSpan, long>
#pragma warning restore SA1402 // File may only contain a single type
	{
		private static readonly Dictionary<DateInterval, (Expression<Func<long, TimeSpan>> from,
			Expression<Func<TimeSpan, long>> to)> TimeSpanExpressions
			= new Dictionary<DateInterval, (Expression<Func<long, TimeSpan>> from, Expression<Func<TimeSpan, long>>
				to)>
			{
				{ DateInterval.Days, (d => TimeSpan.FromDays(d), span => Convert.ToInt64(span.TotalDays)) },
				{ DateInterval.Hours, (d => TimeSpan.FromHours(d), span => Convert.ToInt64(span.TotalHours)) },
				{ DateInterval.Minutes, (d => TimeSpan.FromMinutes(d), span => Convert.ToInt64(span.TotalMinutes)) },
				{ DateInterval.Seconds, (d => TimeSpan.FromSeconds(d), span => Convert.ToInt64(span.TotalSeconds)) },
				{
					DateInterval.Milliseconds,
					(d => TimeSpan.FromMilliseconds(d), span => Convert.ToInt64(span.TotalMilliseconds))
				},
			};

		/// <summary>
		/// Creates a new instance of this converter.
		/// </summary>
		/// <param name="interval"><see cref="DateInterval"/> that the <see cref="long"/> value represents.</param>
		/// <param name="mappingHints">Specifies hints used by the type mapper when using a <see cref="ValueConverter"/>.</param>
		public TimespanToLongConverter(
			DateInterval interval,
			ConverterMappingHints? mappingHints = null)
			: base(TimeSpanExpressions[interval].to, TimeSpanExpressions[interval].from, mappingHints)
		{
		}
	}
}
