namespace Kritikos.Configuration.Persistence.Converters
{
	using System;
	using System.IO;

	using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

	/// <summary>
	/// Handles DirectoryInfo to string conversion (and back) for persistence layers that save only
	/// the relative path.
	/// </summary>
	public class DirectoryInfoToStringConverter : ValueConverter<DirectoryInfo, string>
	{
		/// <summary>
		/// Handles DirectoryInfo to string conversion (and back) for persistence layers that save only relative paths.
		/// </summary>
		/// <param name="relativeBase">The actual folder that relative paths are based on.</param>
		/// <param name="originalSeparator">Path separator character used in string.</param>
		/// <param name="mappingHints">Specifies hints used by the type mapper when using a <see cref="ValueConverter"/>.</param>
		public DirectoryInfoToStringConverter(
			string relativeBase,
			char originalSeparator,
			ConverterMappingHints? mappingHints = null)
			: base(
				v => v.FullName
					.Replace(
						string.IsNullOrWhiteSpace(relativeBase)
							? $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}"
							: relativeBase,
						string.Empty)
					.Replace(Path.DirectorySeparatorChar, originalSeparator),
				v => v.StartsWith(relativeBase, StringComparison.InvariantCulture)
					? new DirectoryInfo(v.Replace(originalSeparator, Path.DirectorySeparatorChar))
					: new DirectoryInfo(
						Path.Combine(relativeBase, v.Replace(originalSeparator, Path.DirectorySeparatorChar))),
				mappingHints)
		{
		}
	}
}
