namespace Kritikos.Configuration.PersistenceTests.ConverterTests
{
  using System.ComponentModel;

  using FluentAssertions;

  using Kritikos.Configuration.Persistence.Converters;

  using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

  using Xunit;

  public class EnumToDescriptionTests
  {
    private static readonly ConverterMappingHints MappingHints = new(unicode: true);

    [Theory]
    [InlineData("None", Foobar.None)]
    [InlineData("Text", Foobar.txt)]
    [InlineData("Music", Foobar.mp3)]
    [InlineData("Video", Foobar.mp4)]
    public void Enum_to_Description(string stringValue, Foobar enumValue)
    {
      var converter = new EnumToDescriptionStringConverter<Foobar>(MappingHints);

      var @enum = (Foobar)converter.ConvertFromProvider(stringValue);
      var description = converter.ConvertToProvider(enumValue) as string;

      @enum.Should().Be(enumValue);
      description.Should().Be(stringValue);
    }
  }

  public enum Foobar
  {
    None,

    [Description("Text")]
    txt,

    [Description("Music")]
    mp3,

    [Description("Video")]
    mp4,
  }
}
