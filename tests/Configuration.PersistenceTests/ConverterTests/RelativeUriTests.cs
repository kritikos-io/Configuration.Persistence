namespace Kritikos.Configuration.PersistenceTests.ConverterTests
{
  using System;

  using FluentAssertions;

  using Kritikos.Configuration.Persistence.Converters;

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
      uri.ToString().Should().Be(AbsoluteUri);

      var str = converter.ConvertToProvider(uri) as string;
      str.Should().Be(Relative);
    }

    [Fact]
    public void Valid_relative_Uri_arbitrary_level()
    {
      var repo = $"{Relative}/Configuration.Persistence";
      var absolute = $"{Base}/{repo}";

      var converter = new RelativeUriToStringConverter(new Uri(Base), MappingHints);

      var uri = converter.ConvertFromProvider(repo) as Uri;
      uri.ToString().Should().Be(absolute);
    }
  }
}
