namespace Kritikos.Configuration.Persistence.Converters.IO;

using System.IO;

/// <summary>
/// Shared path normalisation used by the <see cref="FileInfo"/> and <see cref="DirectoryInfo"/> converters.
/// </summary>
internal static class PathConversion
{
  /// <summary>
  /// Normalises an absolute path into the form persisted by the storage layer.
  /// </summary>
  /// <param name="basePath">Optional root the stored path is expressed relative to.</param>
  /// <param name="info">The file system entry being persisted.</param>
  /// <param name="separator">Character used as directory separator in the persistence layer.</param>
  /// <returns>The path as it should be written to the database.</returns>
  public static string ToStorage(FileSystemInfo? basePath, FileSystemInfo info, char separator)
  {
    var path = info.FullName;

    if (basePath != null)
    {
      path = Path.GetRelativePath(basePath.FullName, path);
    }
    else
    {
      var root = Path.GetPathRoot(path) ?? string.Empty;

      // A drive letter is meaningless once the separator is normalised; UNC roots carry the server and share, so they are kept.
      if (separator != Path.DirectorySeparatorChar
          && root.Length > 2
          && root[1] == Path.VolumeSeparatorChar)
      {
        path = path[(root.Length - 1)..];
      }
    }

    return path.Replace(Path.DirectorySeparatorChar, separator);
  }

  /// <summary>
  /// Rebuilds an absolute path from its persisted form.
  /// </summary>
  /// <param name="basePath">Optional root the stored path is expressed relative to.</param>
  /// <param name="storedPath">The path as read from the database.</param>
  /// <param name="separator">Character used as directory separator in the persistence layer.</param>
  /// <returns>The path in the form used by the local file system.</returns>
  /// <remarks>
  /// <paramref name="basePath"/> is a prefix, not a boundary. A stored path holding <c>..</c> segments resolves above the root, and an absolute one replaces it outright, both of which round trip values <see cref="ToStorage"/> legitimately produces for entries outside the root or on another volume.
  /// </remarks>
  public static string FromStorage(FileSystemInfo? basePath, string storedPath, char separator)
    => (basePath == null
        ? storedPath
        : Path.Combine(basePath.FullName, storedPath))
      .Replace(separator, Path.DirectorySeparatorChar);
}
