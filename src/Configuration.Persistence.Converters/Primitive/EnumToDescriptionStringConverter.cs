namespace Kritikos.Configuration.Persistence.Converters.Primitive;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

/// <summary>
/// Persists an enum as the text of its <see cref="DescriptionAttribute"/>, falling back to the member name.
/// </summary>
/// <remarks>
/// Values outside the ones declared by <typeparamref name="TEnum"/> are written as their numeric text, and stored text matching no member is read back as <c>default</c>, since a value converter cannot reject a value already in the database.
/// When two members share a description the first one declared wins on read.
/// </remarks>
/// <typeparam name="TEnum">The enum being converted.</typeparam>
/// <param name="mappingHints">Hints that can be used by the type mapper to create data types with appropriate facets.</param>
public class EnumToDescriptionStringConverter<TEnum>(ConverterMappingHints? mappingHints = null)
  : ValueConverter<TEnum, string>(
    v => ToDescription(v),
    v => FromDescription(v),
    mappingHints)
  where TEnum : struct, Enum
{
  private static readonly Dictionary<TEnum, string> EnumString = Enum.GetValues<TEnum>()
    .ToDictionary(
      x => x,
      GetDescription);

  private static readonly Dictionary<string, TEnum> DescriptionEnum = EnumString
    .GroupBy(x => x.Value, StringComparer.Ordinal)
    .ToDictionary(x => x.Key, x => x.First().Key, StringComparer.Ordinal);

  private static string ToDescription(TEnum value)
    => EnumString.TryGetValue(value, out var description)
      ? description
      : value.ToString();

  private static TEnum FromDescription(string description)
    => DescriptionEnum.TryGetValue(description, out var member)
      ? member
      : default;

  private static string GetDescription(TEnum value)
    => value.GetType().GetField(value.ToString())?.GetCustomAttribute<DescriptionAttribute>()?.Description
       ?? value.ToString();
}
