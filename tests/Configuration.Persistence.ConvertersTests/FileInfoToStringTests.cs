namespace Kritikos.Configuration.Persistence.ConvertersTests;

using System;
using System.IO;

using Kritikos.Configuration.Persistence.Converters.IO;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using Xunit;

public class FileInfoToStringTests
{
  private const string WindowsBase = @"C:\Windows\System32";
  private const string WindowsRelative = @"drivers\etc\hosts";

  private const string WindowsPath = @"C:\Windows\System32\drivers\etc\hosts";

  private const string LinuxBase = "/srv/http";
  private const string LinuxRelative = "root/index.html";
  private const string LinuxPath = "/srv/http/root/index.html";

  private static readonly ConverterMappingHints MappingHints = new(unicode: true);

  public void Relative_path_windows()
  {
    var converter =
      new FileInfoToStringConverter('\\', new DirectoryInfo(WindowsBase), MappingHints);

    var file = converter.ConvertFromProvider(WindowsRelative) as FileInfo;
    var foo = converter.ConvertToProvider(file) as string;

    Assert.Equal(WindowsPath, file.FullName);
    Assert.Equal(WindowsRelative, foo);
  }

  public void Absolute_path_windows()
  {
    var converter =
      new FileInfoToStringConverter('\\', mappingHints: MappingHints);

    var file = converter.ConvertFromProvider(WindowsPath) as FileInfo;
    var foo = converter.ConvertToProvider(file) as string;

    Assert.Equal(WindowsPath, file.FullName);
    Assert.Equal(WindowsRelative, foo);
  }

  public void Relative_path_nix()
  {
    var converter =
      new FileInfoToStringConverter(
        '/',
        new DirectoryInfo(LinuxBase),
        mappingHints: MappingHints);

    var file = converter.ConvertFromProvider(LinuxRelative) as FileInfo;
    var foo = converter.ConvertToProvider(file) as string;

    Assert.Equal(LinuxRelative, foo);
  }

  public void Absolute_path_nix()
  {
    var converter =
      new FileInfoToStringConverter('/', mappingHints: MappingHints);

    var file = converter.ConvertFromProvider(LinuxPath) as FileInfo;
    var foo = converter.ConvertToProvider(file) as string;

    Assert.Equal(LinuxPath, foo);
  }
}
