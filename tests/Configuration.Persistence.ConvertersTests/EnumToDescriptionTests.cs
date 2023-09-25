// ReSharper disable InconsistentNaming

#pragma warning disable SA1201 // Elements should appear in the correct order
#pragma warning disable SA1402 // File may only contain a single type

namespace Kritikos.Configuration.Persistence.ConvertersTests;

using System.ComponentModel;

using Kritikos.Configuration.Persistence.Converters.Primitive;

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

    var @enum = (Foobar)converter.ConvertFromProvider(stringValue)!;
    var description = converter.ConvertToProvider(enumValue) as string;

    Assert.Equal(enumValue, @enum);
    Assert.Equal(stringValue, description);
  }
}

#pragma warning disable SA1300 // Element should begin with upper-case letter
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
