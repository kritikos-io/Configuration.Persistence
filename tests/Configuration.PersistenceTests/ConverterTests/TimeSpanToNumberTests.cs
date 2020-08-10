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
			var converter = new TimespanToLongConverter(interval, Hints);
			var longFromSpan = converter.ConvertToProviderExpression.Compile();
			var spanFromLong = converter.ConvertFromProviderExpression.Compile();

			var value = Random.NextLong(Convert.ToInt64(Mapping[interval](TimeSpan.MinValue)),
				Convert.ToInt64(Mapping[interval](TimeSpan.MaxValue)));

			var span = spanFromLong(value);
			Assert.Equal(value, Math.Round(Mapping[interval](span), 0));


			var daysReverse = longFromSpan(span);
			Assert.Equal(value, daysReverse);
		}

		[Theory]
		[InlineData(DateInterval.Days)]
		[InlineData(DateInterval.Hours)]
		[InlineData(DateInterval.Minutes)]
		[InlineData(DateInterval.Seconds)]
		[InlineData(DateInterval.Milliseconds)]
		public void DoubleInterval(DateInterval interval)
		{
			var converter = new TimespanToDoubleConverter(interval, Hints);
			var longFromSpan = converter.ConvertToProviderExpression.Compile();
			var spanFromLong = converter.ConvertFromProviderExpression.Compile();

			var value = (double)Random.NextLong(Convert.ToInt64(Mapping[interval](TimeSpan.MinValue)),
				Convert.ToInt64(Mapping[interval](TimeSpan.MaxValue)));

			var span = spanFromLong(value);
			Assert.Equal(Math.Round(value, 0), Math.Round(Mapping[interval](span), 0));


			var reverse = longFromSpan(span);
			Assert.Equal(Math.Round(value, 0), Math.Round(reverse, 0));
		}


		[Theory]
		[InlineData(DateInterval.Days)]
		[InlineData(DateInterval.Hours)]
		[InlineData(DateInterval.Minutes)]
		[InlineData(DateInterval.Seconds)]
		[InlineData(DateInterval.Milliseconds)]
		public void ZeroNumbers(DateInterval interval)
		{
			var converter = new TimespanToDoubleConverter(interval, Hints);
			var longFromSpan = converter.ConvertToProviderExpression.Compile();
			var spanFromLong = converter.ConvertFromProviderExpression.Compile();

			const int value = 0;

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
			var converter = new TimespanToLongConverter(interval, Hints);
			var longFromSpan = converter.ConvertToProviderExpression.Compile();
			var spanFromLong = converter.ConvertFromProviderExpression.Compile();

			var value = Random.NextLong(Convert.ToInt64(Mapping[interval](TimeSpan.MinValue)),
				Convert.ToInt64(Mapping[interval](TimeSpan.MaxValue))) * -1;

			var span = spanFromLong(value);
			Assert.Equal(value, Math.Round(Mapping[interval](span), 0));

			var reverse = longFromSpan(span);
			Assert.Equal(value, reverse);
		}
	}
}
