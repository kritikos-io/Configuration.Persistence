namespace Kritikos.Configuration.Persistence.ConvertersTests;

using System;

using Kritikos.Configuration.Persistence.Converters.Net;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using Xunit;

public class RelativeUriTests
{
  private const string AbsoluteUri = "https://github.com/kritikos-io";
  private const string Base = "https://github.com";
  private const string Relative = "kritikos-io";
  private static readonly ConverterMappingHints MappingHints = new(unicode: true);

  [Fact]
  public void Valid_relative_Uri_single_level()
  {
    var converter = new RelativeUriToStringConverter(new Uri(Base), MappingHints);

    var uri = converter.ConvertFromProvider(Relative) as Uri;
    Assert.Equal(AbsoluteUri, uri!.ToString());

    var str = converter.ConvertToProvider(uri) as string;
    Assert.Equal(Relative, str);
  }

  [Fact]
  public void Valid_relative_Uri_arbitrary_level()
  {
    var repo = $"{Relative}/Configuration.Persistence";
    var absolute = $"{Base}/{repo}";

    var converter = new RelativeUriToStringConverter(new Uri(Base), MappingHints);

    var uri = converter.ConvertFromProvider(repo) as Uri;
    Assert.Equal(absolute, uri!.ToString());
  }
}
