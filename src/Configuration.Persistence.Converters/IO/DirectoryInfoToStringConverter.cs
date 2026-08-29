namespace Kritikos.Configuration.Persistence.Converters.IO;

using System.IO;

using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

/// <summary>
/// Converts from <seealso cref="DirectoryInfo"/> to and from string.
/// </summary>
/// <remarks>When <paramref name="basePath"/> is supplied the stored path is relative to it, including directories outside it, which are stored with leading <c>..</c> segments.</remarks>
/// <param name="separator">Character used as directory separator in the persistence layer.</param>
/// <param name="basePath"><seealso cref="DirectoryInfo"/> used as path base when handling relative paths.</param>
/// <param name="mappingHints">
/// Hints that can be used by the <see cref="ITypeMappingSource" /> to create data types with appropriate
/// facets for the converted data.
/// </param>
public class DirectoryInfoToStringConverter(
  char separator,
  FileSystemInfo? basePath = null,
  ConverterMappingHints? mappingHints = null)
  : ValueConverter<DirectoryInfo, string>(
    v => PathConversion.ToStorage(basePath, v, separator),
    v => new DirectoryInfo(PathConversion.FromStorage(basePath, v, separator)),
    mappingHints);
