namespace Kritikos.Configuration.Persistence.Converters.Primitive
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Linq;
  using System.Reflection;

  using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

  public class EnumToDescriptionStringConverter<TEnum> : ValueConverter<TEnum, string>
    where TEnum : struct, Enum
  {
    private static readonly Dictionary<TEnum, string> EnumString = Enum.GetValues(typeof(TEnum))
      .Cast<TEnum>()
      .ToDictionary(
        x => x,
        GetDescription);

    public EnumToDescriptionStringConverter(ConverterMappingHints? mappingHints = null)
      : base(
        v => EnumString[v],
        v => EnumString.FirstOrDefault(y => y.Value == v).Key,
        mappingHints)
    {
    }

    private static string GetDescription(TEnum value)
      => value.GetType().GetField(value.ToString())?.GetCustomAttribute<DescriptionAttribute>()?.Description
         ?? value.ToString();
  }
}
