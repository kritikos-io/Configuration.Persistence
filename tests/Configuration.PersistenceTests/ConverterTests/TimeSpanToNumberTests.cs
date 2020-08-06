namespace Kritikos.Configuration.PersistenceTests.ConverterTests
{
	using System;
	using System.Collections.Generic;

	using Kritikos.Configuration.Persistence.Converters;
	using Kritikos.Configuration.Persistence.Enums;

	using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

	using Xunit;

	public class TimeSpanToNumberTests
	{
		private static readonly ConverterMappingHints Hints = new ConverterMappingHints(unicode: true);
		private static readonly Random Random = new Random();

		private static readonly Dictionary<DateInterval, Func<TimeSpan, double>> Mapping =
			new Dictionary<DateInterval, Func<TimeSpan, double>>
			{
				{ DateInterval.Days, t => t.TotalDays },
				{ DateInterval.Hours, t => t.TotalHours },
				{ DateInterval.Minutes, t => t.TotalMinutes },
				{ DateInterval.Seconds, t => t.TotalSeconds },
				{ DateInterval.Milliseconds, t => t.TotalMilliseconds },
			};

		[Theory]
		[InlineData(DateInterval.Days)]
		[InlineData(DateInterval.Hours)]
		[InlineData(DateInterval.Minutes)]
		[InlineData(DateInterval.Seconds)]
		[InlineData(DateInterval.Milliseconds)]
		public void LongInterval(DateInterval interval)
		{
			var converter = new TimespanToLongConverter(interval);
			var longFromSpan = converter.ConvertToProviderExpression.Compile();
			var spanFromLong = converter.ConvertFromProviderExpression.Compile();

			var days = Random.Next(0, 100);

			var span = spanFromLong(days);
			Assert.Equal(days, Mapping[interval](span));


			var daysReverse = longFromSpan(span);
			Assert.Equal(days, daysReverse);
		}

		[Theory]
		[InlineData(DateInterval.Days)]
		[InlineData(DateInterval.Hours)]
		[InlineData(DateInterval.Minutes)]
		[InlineData(DateInterval.Seconds)]
		[InlineData(DateInterval.Milliseconds)]
		public void DoubleInterval(DateInterval interval)
		{
			var converter = new TimespanToDoubleConverter(interval);
			var longFromSpan = converter.ConvertToProviderExpression.Compile();
			var spanFromLong = converter.ConvertFromProviderExpression.Compile();

			var value = Random.Next(0, 100);

			var span = spanFromLong(value);
			Assert.Equal(value, Mapping[interval](span));


			var reverse = longFromSpan(span);
			Assert.Equal(value, reverse);
		}


		[Theory]
		[InlineData(DateInterval.Days)]
		[InlineData(DateInterval.Hours)]
		[InlineData(DateInterval.Minutes)]
		[InlineData(DateInterval.Seconds)]
		[InlineData(DateInterval.Milliseconds)]
		public void ZeroNumbers(DateInterval interval)
		{
			var converter = new TimespanToDoubleConverter(interval);
			var longFromSpan = converter.ConvertToProviderExpression.Compile();
			var spanFromLong = converter.ConvertFromProviderExpression.Compile();

			var value = 0;

			var span = spanFromLong(value);
			Assert.Equal(value, Mapping[interval](span));

			var reverse = longFromSpan(span);
			Assert.Equal(value, reverse);
		}

		[Theory]
		[InlineData(DateInterval.Days)]
		[InlineData(DateInterval.Hours)]
		[InlineData(DateInterval.Minutes)]
		[InlineData(DateInterval.Seconds)]
		[InlineData(DateInterval.Milliseconds)]
		public void Negative(DateInterval interval)
		{
			var converter = new TimespanToDoubleConverter(interval);
			var longFromSpan = converter.ConvertToProviderExpression.Compile();
			var spanFromLong = converter.ConvertFromProviderExpression.Compile();

			var value = Random.Next() * -1;
			var span = spanFromLong(value);
			Assert.Equal(value, Mapping[interval](span));

			var reverse = longFromSpan(span);
			Assert.Equal(value, reverse);
		}
	}
}
