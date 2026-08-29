namespace Kritikos.Configuration.Persistence.Converters.IO;

using System.IO;

using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

/// <summary>
/// Converts from <seealso cref="DirectoryInfo"/> to and from string.
/// </summary>
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
    v => FromDirectoryInfo(basePath, v, separator),
    v => FromPath(basePath, v, separator),
    mappingHints)
{
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
    var rootPath = Path.GetPathRoot(path) ?? string.Empty;

    if (basePath != null)
    {
      path = path.Replace(basePath.FullName, string.Empty, StringComparison.InvariantCulture)[1..];
    }
    else if (separator != '\\' && Path.DirectorySeparatorChar == '\\' && rootPath.Length - 1 > 0)
    {
      path = path[(rootPath.Length - 1)..];
    }

    return path.Replace(Path.DirectorySeparatorChar, separator);
  }
}
