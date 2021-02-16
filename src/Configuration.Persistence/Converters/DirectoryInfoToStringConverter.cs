namespace Kritikos.Configuration.Persistence.Converters
{
	using System.IO;

	using Microsoft.EntityFrameworkCore.Storage;
	using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

	/// <summary>
	/// Converts from <seealso cref="DirectoryInfo"/> to and from string.
	/// </summary>
	public class DirectoryInfoToStringConverter : ValueConverter<DirectoryInfo, string>
	{
		/// <summary>
		/// Initializes a new instance of the <seealso cref="DirectoryInfoToStringConverter"/> class.
		/// </summary>
		/// <param name="separator">Character used as directory separator in the persistence layer.</param>
		/// <param name="basePath"><seealso cref="DirectoryInfo"/> used as path base when handling relative paths.</param>
		/// <param name="mappingHints">
		/// Hints that can be used by the <see cref="ITypeMappingSource" /> to create data types with appropriate
		/// facets for the converted data.
		/// </param>
		public DirectoryInfoToStringConverter(
			char separator,
			FileSystemInfo? basePath = null,
			ConverterMappingHints? mappingHints = null)
			: base(
				v => FromDirectoryInfo(basePath, v, separator),
				v => FromPath(basePath, v, separator),
				mappingHints)
		{
		}

		private static DirectoryInfo FromPath(FileSystemInfo? basePath, string directoryPath, char separator)
		{
			var path = (basePath == null
					? directoryPath
					: Path.Combine(basePath.FullName, directoryPath))
				.Replace(separator, Path.DirectorySeparatorChar);

			return new DirectoryInfo(path);
		}

		private static string FromDirectoryInfo(FileSystemInfo? basePath, FileSystemInfo directory, char separator)
		{
			var path = directory.FullName;
			path = basePath != null
				? path.Replace(basePath?.FullName ?? string.Empty, string.Empty)[1..]
				: path;

			path = separator != '\\' && Path.DirectorySeparatorChar == '\\' && Path.GetPathRoot(path).Length - 1 > 0
				? path[(Path.GetPathRoot(path).Length - 1)..]
				: path;
			path = path.Replace(Path.DirectorySeparatorChar, separator);

			return path;
		}
	}
}
