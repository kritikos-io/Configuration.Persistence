namespace Kritikos.Configuration.PersistenceTests.ConverterTests.TimeSpanToNumberTests
{
	using System;

	using Kritikos.Configuration.Persistence.Converters;
	using Kritikos.Configuration.Persistence.Enums;

	using Xunit;

	public class TimeSpanToLongTests : TimeSpanToNumberConverterTests<long>
	{
		private static new readonly Func<TimeSpan, DateInterval, long> FromTimespan = ((span, interval) =>
			Convert.ToInt64(TimeSpanToNumberConverterTests.FromTimespan(span, interval)));

		private static new readonly Func<long, DateInterval, TimeSpan> ToTimeSpan = ((value, interval) =>
			TimeSpanToNumberConverterTests.ToTimeSpan(Convert.ToDouble(value), interval));

		/// <inheritdoc />
		protected override TimeSpanToLongConverter CreateConverter(DateInterval interval) => new(interval, MappingHints);

		[Fact]
		public void Check_TimeSpan_in_total_days()
		{
			const DateInterval interval = DateInterval.Days;
			Tester(TimeSpan.MaxValue, ToTimeSpan, FromTimespan, interval);
			Tester(TimeSpan.MinValue, ToTimeSpan, FromTimespan, interval);
		}

		[Fact]
		public void Check_TimeSpan_in_total_hours()
		{
			const DateInterval interval = DateInterval.Hours;
			Tester(TimeSpan.MaxValue.Subtract(TimeSpan.FromHours(1)), ToTimeSpan, FromTimespan, interval);
			Tester(TimeSpan.MinValue.Add(TimeSpan.FromHours(1)), ToTimeSpan, FromTimespan, interval);
		}

		[Fact]
		public void Check_TimeSpan_in_total_minutes()
		{
			const DateInterval interval = DateInterval.Minutes;
			Tester(TimeSpan.MaxValue.Subtract(TimeSpan.FromHours(1)), ToTimeSpan, FromTimespan, interval);
			Tester(TimeSpan.MinValue.Add(TimeSpan.FromHours(1)), ToTimeSpan, FromTimespan, interval);
		}

		[Fact]
		public void Check_TimeSpan_in_total_seconds()
		{
			const DateInterval interval = DateInterval.Seconds;
			Tester(TimeSpan.MaxValue.Subtract(TimeSpan.FromHours(1)), ToTimeSpan, FromTimespan, interval);
			Tester(TimeSpan.MinValue.Add(TimeSpan.FromHours(1)), ToTimeSpan, FromTimespan, interval);
		}

		[Fact]
		public void Check_TimeSpan_in_total_milliseconds()
		{
			const DateInterval interval = DateInterval.Milliseconds;
			Tester(TimeSpan.MaxValue.Subtract(TimeSpan.FromHours(1)), ToTimeSpan, FromTimespan, interval);
			Tester(TimeSpan.MinValue.Add(TimeSpan.FromHours(1)), ToTimeSpan, FromTimespan, interval);
		}

		[Fact]
		public void Check_TimeSpan_in_ticks()
		{
			const DateInterval interval = DateInterval.Ticks;
			Tester(TimeSpan.MaxValue.Subtract(TimeSpan.FromHours(1)), ToTimeSpan, FromTimespan, interval);
			Tester(TimeSpan.MinValue.Add(TimeSpan.FromHours(1)), ToTimeSpan, FromTimespan, interval);
		}
	}
}
