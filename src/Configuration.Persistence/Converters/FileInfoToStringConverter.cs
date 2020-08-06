namespace Kritikos.Configuration.Persistence.Converters
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

	/// <summary>
	/// Handles <see cref="FileInfo"/> to <see cref="string"/> conversion (and back) for persistence layers with EF Core.
	/// the relative path.
	/// </summary>
	public class FileInfoToStringConverter : ValueConverter<FileInfo, string>
	{
		private static readonly Dictionary<string, Dictionary<string, string>> Substitutions
			= new Dictionary<string, Dictionary<string, string>>();

		private static readonly Dictionary<string, Dictionary<string, string>> ReverseSubstitutions
			= new Dictionary<string, Dictionary<string, string>>();

		private static readonly Dictionary<string, string> RelativeBases
			= new Dictionary<string, string>();

		private static readonly Dictionary<string, char> OriginalSeperators
			= new Dictionary<string, char>();

		/// <summary>
		/// Handles <see cref="FileInfo"/> to <see cref="string"/> conversion (and back) for persistence layers with EF Core.
		/// </summary>
		/// <param name="mappingHints">Specifies hints used by the type mapper when using a <see cref="ValueConverter"/>.</param>
		/// <param name="name">Instance name, to allow substitution lookups.</param>
		/// <param name="relativeBase">The actual folder that relative paths are based on.</param>
		/// <param name="originalSeparator">Path separator character used in string.</param>
		/// <param name="substitutions">A map of folder names that should be mapped to different folders based on the executing machine.</param>
		/// <remarks>When using relative paths, always provide <paramref name="relativeBase"/>, even if it is a dummy value.</remarks>
		/// <exception cref="ArgumentException"><paramref name="substitutions"/> contains duplicate values.</exception>
		public FileInfoToStringConverter(
			ConverterMappingHints mappingHints,
			string name,
			string relativeBase,
			char originalSeparator,
			Dictionary<string, string> substitutions)
			: base(
				file => DeconstructFile(file, name),
				path => ConstructPath(path, name),
				mappingHints)
		{
			if (Substitutions.ContainsKey(name))
			{
				throw new ArgumentException("Duplicate converter registration!", nameof(name));
			}

			Substitutions.TryAdd(name, substitutions);
			ReverseSubstitutions.TryAdd(name, substitutions.ToDictionary(x => x.Value, x => x.Key));
			OriginalSeperators.TryAdd(name, originalSeparator);
			RelativeBases.TryAdd(name, relativeBase);
		}

		private static string DeconstructFile(
			FileSystemInfo file,
			string name)
		{
			var filePath = file.FullName;
			{
				var relativeBase = RelativeBases[name];
				if (!string.IsNullOrWhiteSpace(relativeBase))
				{
					filePath = filePath.Replace(relativeBase, string.Empty, StringComparison.InvariantCulture);
				}
			}

			{
				var (substitute, original) = ReverseSubstitutions[name]
					.Where(x => filePath.StartsWith(x.Key, StringComparison.InvariantCulture))
					.OrderByDescending(x => x.Key.Length)
					.FirstOrDefault();

				if (!string.IsNullOrWhiteSpace(substitute) || !string.IsNullOrWhiteSpace(original))
				{
					filePath = filePath.Replace(substitute, original, StringComparison.InvariantCulture);
				}
			}

			return filePath.Replace(Path.DirectorySeparatorChar, OriginalSeperators[name]);
		}

		private static FileInfo ConstructPath(
			string path,
			string name)
		{
			{
				var (originalPath, substitution) = Substitutions[name]
					.Where(x => path.StartsWith(x.Key, StringComparison.InvariantCulture))
					.OrderByDescending(x => x.Key.Length)
					.FirstOrDefault();

				if (!string.IsNullOrWhiteSpace(originalPath) || !string.IsNullOrWhiteSpace(substitution))
				{
					path = path.Replace(originalPath, substitution, StringComparison.InvariantCulture);
				}
			}

			{
				var relativeBase = RelativeBases[name];
				if (!string.IsNullOrWhiteSpace(relativeBase))
				{
					path = Path.Combine(relativeBase, path);
				}
			}

			path = path.Replace(OriginalSeperators[name], Path.DirectorySeparatorChar);

			return new FileInfo(path);
		}
	}
}
