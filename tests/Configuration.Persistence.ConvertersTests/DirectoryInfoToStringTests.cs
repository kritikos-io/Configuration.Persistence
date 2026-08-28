namespace Kritikos.Configuration.Persistence.ConvertersTests;

using System.IO;

using Kritikos.Configuration.Persistence.Converters.IO;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using TUnit.Core.Enums;

public class DirectoryInfoToStringTests
{
  private const string WindowsBase = @"C:\Windows\System32";
  private const string WindowsRelative = @"drivers\etc";
  private const string WindowsPath = @"C:\Windows\System32\drivers\etc";

  private const string LinuxBase = "/srv/http";
  private const string LinuxRelative = "root";
  private const string LinuxPath = "/srv/http/root";

  private static readonly ConverterMappingHints MappingHints = new(unicode: true);

  [Test]
  [RunOn(OS.Windows)]
  public async Task Relative_path_windows()
  {
    var converter =
        new DirectoryInfoToStringConverter('\\', new DirectoryInfo(WindowsBase), MappingHints);

    var file = converter.ConvertFromProvider(WindowsRelative) as DirectoryInfo;
    var foo = converter.ConvertToProvider(file) as string;

    await Assert.That(file?.FullName).IsEqualTo(WindowsPath);
    await Assert.That(foo).IsEqualTo(WindowsRelative);
  }

  [Test]
  [RunOn(OS.Windows)]
  public async Task Absolute_path_windows()
  {
    var converter =
        new DirectoryInfoToStringConverter('\\', mappingHints: MappingHints);

    var file = converter.ConvertFromProvider(WindowsPath) as DirectoryInfo;
    var foo = converter.ConvertToProvider(file) as string;

    await Assert.That(file?.FullName).IsEqualTo(WindowsPath);

    // Without a base path there is nothing to make the result relative against.
    await Assert.That(foo).IsEqualTo(WindowsPath);
  }

  [Test]
  public async Task Relative_path_linux()
  {
    var converter = new DirectoryInfoToStringConverter(
        '/',
        new DirectoryInfo(LinuxBase),
        mappingHints: MappingHints);

    var file = converter.ConvertFromProvider(LinuxRelative) as DirectoryInfo;
    var foo = converter.ConvertToProvider(file) as string;

    await Assert.That(foo).IsEqualTo(LinuxRelative);
  }

  [Test]
  public async Task Absolute_path_linux()
  {
    var converter =
        new DirectoryInfoToStringConverter('/', mappingHints: MappingHints);

    var file = converter.ConvertFromProvider(LinuxPath) as DirectoryInfo;
    var foo = converter.ConvertToProvider(file) as string;

    await Assert.That(foo).IsEqualTo(LinuxPath);
  }
}
