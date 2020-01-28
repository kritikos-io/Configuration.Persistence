namespace Kritikos.Configuration.Persistence.Converters
{
	using System;
	using System.IO;

	using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

	public class DirectoryInfoToStringConverter : ValueConverter<DirectoryInfo, string>
	{
		public DirectoryInfoToStringConverter(
			string baseFolder,
			char originalSeperator,
			ConverterMappingHints? mappingHints = null)
			: base(
				v => v.ToString()
					.Replace(
						string.IsNullOrWhiteSpace(baseFolder)
							? Directory.GetCurrentDirectory()
							: baseFolder,
						string.Empty)
					.Replace(Path.DirectorySeparatorChar, originalSeperator),
				v => v.StartsWith(baseFolder, StringComparison.InvariantCulture)
					? new DirectoryInfo(v.Replace(originalSeperator, Path.DirectorySeparatorChar))
					: new DirectoryInfo(
						Path.Combine(baseFolder, v.Replace(originalSeperator, Path.DirectorySeparatorChar))),
				mappingHints)
		{
		}
	}
}
