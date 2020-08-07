namespace Kritikos.Configuration.PersistenceTests.ConverterTests
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	using Kritikos.Configuration.Persistence.Converters;

	using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

	using Xunit;

	public class StringListToStringTests
	{
		private static readonly ConverterMappingHints Hints = new ConverterMappingHints(unicode: true);

		[Fact]
		public void StringListToString()
		{
			var foo = new List<string> { "Rock", "Country", "Folk", };

			var converter =
				new StringListToDelimitedStringConverter(",", Hints);

			var listToString = converter.ConvertToProviderExpression.Compile();
			var stringToList = converter.ConvertFromProviderExpression.Compile();

			var delimited = listToString(foo);
			foreach (var genre in foo)
			{
				Assert.Contains(genre, delimited);
			}

			var genres = stringToList(delimited);
			Assert.Equal(genres.Count, foo.Count);

			foreach (var genre in genres)
			{
				Assert.Contains(genre, foo);
			}
		}

		[Fact]
		public void EmptyList()
		{
			var foo = new List<string>();

			var converter =
				new StringListToDelimitedStringConverter(",", Hints);
			var listToString = converter.ConvertToProviderExpression.Compile();
			var stringToList = converter.ConvertFromProviderExpression.Compile();

			var delimited = listToString(foo);

			Assert.True(string.IsNullOrWhiteSpace(delimited));

			var reverse = stringToList(delimited).ToList();
			Assert.Empty(reverse);
		}
	}
}
