namespace Kritikos.Configuration.Persistence.Converters.Net;

using System;

using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

/// <summary>
/// Converts relative <seealso cref="Uri"/> to <seealso cref="string"/>.
/// </summary>
/// <remarks>Strings that cannot be combined with the base <seealso cref="Uri"/> into a valid <seealso cref="Uri"/> are read back as <c>about:blank</c>.</remarks>
public class RelativeUriToStringConverter : ValueConverter<Uri, string>
{
  private static readonly Uri Fallback = new("about:blank");

  /// <summary>
  /// Initializes a new instance of the <see cref="RelativeUriToStringConverter"/> class.
  /// </summary>
  /// <param name="baseUri">The base of relative <seealso cref="Uri"/> to be constructed.</param>
  /// <param name="mappingHints">
  /// Hints that can be used by the <see cref="ITypeMappingSource" /> to create data types with appropriate
  /// facets for the converted data.
  /// </param>
  /// <exception cref="ArgumentNullException"><paramref name="baseUri"/> is null.</exception>
  public RelativeUriToStringConverter(Uri baseUri, ConverterMappingHints? mappingHints = null)
    : base(
      v => baseUri.MakeRelativeUri(v).ToString(),
      v => FromString(baseUri, v),
      mappingHints)
    => ArgumentNullException.ThrowIfNull(baseUri);

  private static Uri FromString(Uri baseUri, string relative)
    => Uri.TryCreate(baseUri, relative, out var uri)
      ? uri
      : Fallback;
}
