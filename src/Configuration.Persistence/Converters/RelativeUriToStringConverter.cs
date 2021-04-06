namespace Kritikos.Configuration.Persistence.Converters
{
  using System;

  using Microsoft.EntityFrameworkCore.Storage;
  using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

  /// <summary>
  /// Converts relative <seealso cref="Uri"/> to <seealso cref="string"/>.
  /// </summary>
  public class RelativeUriToStringConverter : ValueConverter<Uri, string>
  {
    private static readonly Uri Fallback = new("about:blank");

    /// <summary>
    /// Converts relative <seealso cref="Uri"/> to <seealso cref="string"/>.
    /// </summary>
    /// <param name="baseUri">The base of relative <seealso cref="Uri"/> to be constructed.</param>
    /// <param name="mappingHints">
    /// Hints that can be used by the <see cref="ITypeMappingSource" /> to create data types with appropriate
    /// facets for the converted data.
    /// </param>
    public RelativeUriToStringConverter(
      Uri baseUri,
      ConverterMappingHints? mappingHints = null)
      : base(
        v => baseUri.MakeRelativeUri(v).ToString(),
        v => FromString(baseUri, v),
        mappingHints)
    {
    }

    private static Uri FromString(Uri baseUri, string relative)
      => Uri.TryCreate(baseUri, relative, out var uri)
        ? uri
        : Fallback;
  }
}
