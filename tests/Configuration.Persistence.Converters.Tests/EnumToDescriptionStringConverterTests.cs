// ReSharper disable InconsistentNaming

#pragma warning disable SA1201 // Elements should appear in the correct order
#pragma warning disable SA1402 // File may only contain a single type

namespace Kritikos.Configuration.Persistence.Converters.Tests;

using System.ComponentModel;

using Kritikos.Configuration.Persistence.Converters.Primitive;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class EnumToDescriptionStringConverterTests
{
  private static readonly ConverterMappingHints MappingHints = new(unicode: true);

  [Test]
  [Arguments("None", Foobar.None)]
  [Arguments("Text", Foobar.txt)]
  [Arguments("Music", Foobar.mp3)]
  [Arguments("Video", Foobar.mp4)]
  public async Task Convert_EnumWithDescription_RoundTripsThroughDescriptionText(string stringValue, Foobar enumValue)
  {
    var converter = new EnumToDescriptionStringConverter<Foobar>(MappingHints);

    var @enum = (Foobar)converter.ConvertFromProvider(stringValue)!;
    var description = converter.ConvertToProvider(enumValue) as string;

    await Assert.That(@enum).IsEqualTo(enumValue);
    await Assert.That(description).IsEqualTo(stringValue);
  }

  [Test]
  public async Task ConvertFromProvider_TextMatchingNoMember_ReturnsDefault()
  {
    var converter = new EnumToDescriptionStringConverter<Foobar>(MappingHints);

    var @enum = (Foobar)converter.ConvertFromProvider("Spreadsheet")!;

    await Assert.That(@enum).IsEqualTo(default(Foobar));
  }

  [Test]
  public async Task ConvertToProvider_ValueOutsideTheEnum_ReturnsNumericText()
  {
    var converter = new EnumToDescriptionStringConverter<Foobar>(MappingHints);

    var description = converter.ConvertToProvider((Foobar)42) as string;

    await Assert.That(description).IsEqualTo("42");
  }
}

#pragma warning disable SA1300 // Element should begin with upper-case letter
public enum Foobar
{
  /// <summary>A member without a description, which falls back to its own name.</summary>
  None,

  /// <summary>A member whose description differs from its name.</summary>
  [Description("Text")]
  txt,

  /// <summary>A member whose description differs from its name.</summary>
  [Description("Music")]
  mp3,

  /// <summary>A member whose description differs from its name.</summary>
  [Description("Video")]
  mp4,
}
