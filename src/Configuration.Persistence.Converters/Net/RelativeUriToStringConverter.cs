namespace Kritikos.Configuration.Persistence.Converters.Net;

using System;

using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

/// <summary>
/// Converts relative <seealso cref="Uri"/> to <seealso cref="string"/>.
/// </summary>
/// <remarks>
/// The stored form is relative, the converted property is not: values are absolute <seealso cref="Uri"/> instances expressed relative to the base while stored.
/// Strings that cannot be combined with the base <seealso cref="Uri"/> into a valid <seealso cref="Uri"/> are read back as <c>about:blank</c>.
/// </remarks>
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
      v => ToString(baseUri, v),
      v => FromString(baseUri, v),
      mappingHints)
    => ArgumentNullException.ThrowIfNull(baseUri);

  private static string ToString(Uri baseUri, Uri value)
    => value.IsAbsoluteUri
      ? baseUri.MakeRelativeUri(value).ToString()
      : throw new ArgumentException(
        $"'{value}' is relative, and only an absolute {nameof(Uri)} can be expressed relative to '{baseUri}'.",
        nameof(value));

  private static Uri FromString(Uri baseUri, string relative)
    => Uri.TryCreate(baseUri, relative, out var uri)
      ? uri
      : Fallback;
}
