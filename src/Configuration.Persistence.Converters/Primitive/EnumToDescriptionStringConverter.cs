namespace Kritikos.Configuration.Persistence.Converters.Primitive;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class EnumToDescriptionStringConverter<TEnum>(ConverterMappingHints? mappingHints = null)
  : ValueConverter<TEnum, string>(
    v => EnumString[v],
    v => EnumString.FirstOrDefault(y => y.Value == v).Key,
    mappingHints)
  where TEnum : struct, Enum
{
  private static readonly Dictionary<TEnum, string> EnumString = Enum.GetValues(typeof(TEnum))
    .Cast<TEnum>()
    .ToDictionary(
      x => x,
      GetDescription);

  private static string GetDescription(TEnum value)
    => value.GetType().GetField(value.ToString())?.GetCustomAttribute<DescriptionAttribute>()?.Description
       ?? value.ToString();
}
