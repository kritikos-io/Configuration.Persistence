namespace Kritikos.Configuration.Persistence.Converters
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Linq;
  using System.Reflection;
  using System.Security.Cryptography.X509Certificates;

  using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

  public class EnumToDescriptionStringConverter<TEnum> : ValueConverter<TEnum, string>
    where TEnum : struct, Enum
  {
    private static readonly Dictionary<TEnum, string> EnumString;

    static EnumToDescriptionStringConverter()
    {
      var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

      EnumString = values?.ToDictionary(
                     x => x,
                     GetDescription)
                   ?? new Dictionary<TEnum, string>();
    }

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
