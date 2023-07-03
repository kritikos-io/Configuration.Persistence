namespace Kritikos.Configuration.Persistence.Converters.IO;

using System.IO;

using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

/// <summary>
/// Converts from <seealso cref="FileInfo"/> to and from string.
/// </summary>
public class FileInfoToStringConverter : ValueConverter<FileInfo, string>
{
  /// <summary>
  /// Initializes a new instance of the <seealso cref="FileInfoToStringConverter"/> class.
  /// </summary>
  /// <param name="separator">Character used as directory separator in the persistence layer.</param>
  /// <param name="basePath"><seealso cref="DirectoryInfo"/> used as path base when handling relative paths.</param>
  /// <param name="mappingHints">
  /// Hints that can be used by the <see cref="ITypeMappingSource" /> to create data types with appropriate
  /// facets for the converted data.
  /// </param>
  public FileInfoToStringConverter(
    char separator,
    FileSystemInfo? basePath = null,
    ConverterMappingHints? mappingHints = null)
    : base(
      v => FromFileInfo(basePath, v, separator),
      v => FromPath(basePath, v, separator),
      mappingHints)
  {
  }

  private static FileInfo FromPath(FileSystemInfo? basePath, string filePath, char separator)
  {
    var path = (basePath == null
        ? filePath
        : Path.Combine(basePath.FullName, filePath))
      .Replace(separator, Path.DirectorySeparatorChar);

    return new FileInfo(path);
  }

  private static string FromFileInfo(FileSystemInfo? basePath, FileSystemInfo file, char separator)
  {
    var path = file.FullName;
    var rootPath = Path.GetPathRoot(path) ?? string.Empty;

    path = basePath != null
      ? path
        .Replace(basePath.FullName, string.Empty, StringComparison.InvariantCulture)[1..]
      : path;

    path = separator != '\\' && Path.DirectorySeparatorChar == '\\' && rootPath.Length - 1 > 0
      ? path[(rootPath.Length - 1)..]
      : path;
    path = path.Replace(Path.DirectorySeparatorChar, separator);

    return path;
  }
}
