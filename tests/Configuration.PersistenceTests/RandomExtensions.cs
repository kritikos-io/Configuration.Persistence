namespace Kritikos.Configuration.PersistenceTests
{
	using System;

	public static class RandomExtensions
	{
		public static long NextLong(this Random random, long min, long max)
		{
			if (random == null)
			{
				throw new ArgumentNullException(nameof(random));
			}

			if (max < min)
			{
				throw new ArgumentOutOfRangeException(
					nameof(max),
					$"{nameof(max)} should be greater than {nameof(min)}!");
			}

			var uRange = (ulong)(max - min);

			ulong ulongRand;
			do
			{
				var buffer = new byte[8];
				random.NextBytes(buffer);
				ulongRand = (ulong)BitConverter.ToInt64(buffer, 0);
			}
			while (ulongRand > ulong.MaxValue - (((ulong.MaxValue % uRange) + 1) % uRange));

			return (long)(ulongRand % uRange) + min;
		}

	}
}
