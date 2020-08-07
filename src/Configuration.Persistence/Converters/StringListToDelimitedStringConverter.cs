namespace Kritikos.Configuration.Persistence.Converters
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;

	using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

	public class StringListToDelimitedStringConverter : ValueConverter<List<string>, string>
	{
		/// <summary>
		/// Creates a new instance of this converter.
		/// </summary>
		/// <param name="mappingHints">Specifies hints used by the type mapper when using a <see cref="ValueConverter"/>.</param>
		/// <param name="delimiter"><see cref="string"/> to use in splitting and joining during conversions.</param>
		public StringListToDelimitedStringConverter(string delimiter, ConverterMappingHints? mappingHints = null)
			: base(
				list => string.Join(delimiter, list),
				str => str.Split(delimiter, StringSplitOptions.None)
					.Where(x => !string.IsNullOrWhiteSpace(x))
					.ToList(),
				mappingHints)
		{
		}
	}
}
