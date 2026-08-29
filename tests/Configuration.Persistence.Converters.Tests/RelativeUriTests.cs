namespace Kritikos.Configuration.Persistence.Converters.Tests;

using System;

using Kritikos.Configuration.Persistence.Converters.Net;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class RelativeUriTests
{
  private const string AbsoluteUri = "https://github.com/kritikos-io";
  private const string Base = "https://github.com";
  private const string Relative = "kritikos-io";
  private static readonly ConverterMappingHints MappingHints = new(unicode: true);

  [Test]
  public async Task Convert_SingleSegmentRelativeUri_RoundTripsAgainstBaseUri()
  {
    var converter = new RelativeUriToStringConverter(new Uri(Base), MappingHints);

    var uri = converter.ConvertFromProvider(Relative) as Uri;
    await Assert.That(uri?.ToString()).IsEqualTo(AbsoluteUri);

    var str = converter.ConvertToProvider(uri) as string;
    await Assert.That(str).IsEqualTo(Relative);
  }

  [Test]
  public async Task ConvertFromProvider_MultiSegmentRelativeUri_ReturnsAbsoluteUri()
  {
    var repo = $"{Relative}/Configuration.Persistence";
    var absolute = $"{Base}/{repo}";

    var converter = new RelativeUriToStringConverter(new Uri(Base), MappingHints);

    var uri = converter.ConvertFromProvider(repo) as Uri;
    await Assert.That(uri?.ToString()).IsEqualTo(absolute);
  }
}
