namespace Kritikos.Configuration.Persistence.ConvertersTests;

using System;
using System.IO;

using Kritikos.Configuration.Persistence.Converters.IO;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using Xunit;

public class DirectoryInfoToStringTests
{
  private const string WindowsBase = @"C:\Windows\System32";
  private const string WindowsRelative = @"drivers\etc";
  private const string WindowsPath = @"C:\Windows\System32\drivers\etc";

  private const string LinuxBase = "/srv/http";
  private const string LinuxRelative = "root";
  private const string LinuxPath = "/srv/http/root";

  private static readonly ConverterMappingHints MappingHints = new(unicode: true);

  [SkippableFact]
  public void Relative_path_windows()
  {
    Skip.If(true);
    Skip.If(
      Environment.OSVersion.Platform != PlatformID.Win32NT,
      "Environment.OSVersion.Platform != PlatformID.Win32NT");

    var converter =
      new DirectoryInfoToStringConverter('\\', new DirectoryInfo(WindowsBase), MappingHints);

    var file = converter.ConvertFromProvider(WindowsRelative) as DirectoryInfo;
    var foo = converter.ConvertToProvider(file) as string;

    Assert.Equal(WindowsPath, file.FullName);
    Assert.Equal(WindowsRelative, foo);
  }

  [SkippableFact]
  public void Absolute_path_windows()
  {
    Skip.If(true);
    Skip.If(
      Environment.OSVersion.Platform != PlatformID.Win32NT,
      "Environment.OSVersion.Platform != PlatformID.Win32NT");

    var converter =
      new DirectoryInfoToStringConverter('\\', mappingHints: MappingHints);

    var file = converter.ConvertFromProvider(WindowsPath) as DirectoryInfo;
    var foo = converter.ConvertToProvider(file) as string;

    Assert.Equal(WindowsPath, file.FullName);
    Assert.Equal(WindowsRelative, foo);
  }

  [SkippableFact]
  public void Relative_path_linux()
  {
    Skip.If(true);
    Skip.If(
      Environment.OSVersion.Platform == PlatformID.Win32NT,
      "Environment.OSVersion.Platform == PlatformID.Win32NT");

    var converter =
      new DirectoryInfoToStringConverter(
        '/',
        new DirectoryInfo(LinuxBase),
        mappingHints: MappingHints);

    var file = converter.ConvertFromProvider(LinuxRelative) as DirectoryInfo;
    var foo = converter.ConvertToProvider(file) as string;

    Assert.Equal(LinuxRelative, foo);
  }

  [SkippableFact]
  public void Absolute_path_linux()
  {
    Skip.If(true);
    Skip.If(
      Environment.OSVersion.Platform == PlatformID.Win32NT,
      "Environment.OSVersion.Platform == PlatformID.Win32NT");

    var converter =
      new DirectoryInfoToStringConverter('/', mappingHints: MappingHints);

    var file = converter.ConvertFromProvider(LinuxPath) as DirectoryInfo;
    var foo = converter.ConvertToProvider(file) as string;

    Assert.Equal(LinuxPath, foo);
  }
}
