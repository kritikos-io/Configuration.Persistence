namespace Kritikos.Configuration.Persistence.Converters.Tests;

using System.IO;

using Kritikos.Configuration.Persistence.Converters.IO;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using TUnit.Core.Enums;

public class FileInfoToStringConverterTests
{
  private const string WindowsBase = @"C:\Windows\System32";
  private const string WindowsRelative = @"drivers\etc\hosts";

  private const string WindowsPath = @"C:\Windows\System32\drivers\etc\hosts";

  private const string LinuxBase = "/srv/http";
  private const string LinuxRelative = "root/index.html";
  private const string LinuxPath = "/srv/http/root/index.html";

  private static readonly ConverterMappingHints MappingHints = new(unicode: true);

  [Test]
  [RunOn(OS.Windows)]
  public async Task Convert_WindowsRelativePathWithBasePath_RoundTripsToRelativePath()
  {
    var converter =
      new FileInfoToStringConverter('\\', new DirectoryInfo(WindowsBase), MappingHints);

    var file = converter.ConvertFromProvider(WindowsRelative) as FileInfo;
    var foo = converter.ConvertToProvider(file) as string;

    await Assert.That(file?.FullName).IsEqualTo(WindowsPath);
    await Assert.That(foo).IsEqualTo(WindowsRelative);
  }

  [Test]
  [RunOn(OS.Windows)]
  public async Task Convert_WindowsAbsolutePathWithoutBasePath_RoundTripsToAbsolutePath()
  {
    var converter =
      new FileInfoToStringConverter('\\', mappingHints: MappingHints);

    var file = converter.ConvertFromProvider(WindowsPath) as FileInfo;
    var foo = converter.ConvertToProvider(file) as string;

    await Assert.That(file?.FullName).IsEqualTo(WindowsPath);

    // Without a base path there is nothing to make the result relative against.
    await Assert.That(foo).IsEqualTo(WindowsPath);
  }

  [Test]
  public async Task Convert_UnixRelativePathWithBasePath_RoundTripsToRelativePath()
  {
    var converter =
      new FileInfoToStringConverter(
        '/',
        new DirectoryInfo(LinuxBase),
        mappingHints: MappingHints);

    var file = converter.ConvertFromProvider(LinuxRelative) as FileInfo;
    var foo = converter.ConvertToProvider(file) as string;

    await Assert.That(foo).IsEqualTo(LinuxRelative);
  }

  [Test]
  public async Task Convert_UnixAbsolutePathWithoutBasePath_RoundTripsToAbsolutePath()
  {
    var converter =
      new FileInfoToStringConverter('/', mappingHints: MappingHints);

    var file = converter.ConvertFromProvider(LinuxPath) as FileInfo;
    var foo = converter.ConvertToProvider(file) as string;

    await Assert.That(foo).IsEqualTo(LinuxPath);
  }

  [Test]
  [RunOn(OS.Windows)]
  public async Task ConvertToProvider_PathOutsideTheBasePath_UsesParentSegments()
  {
    var converter =
      new FileInfoToStringConverter('\\', new DirectoryInfo(WindowsBase), MappingHints);

    var stored = converter.ConvertToProvider(new FileInfo(@"C:\Windows\notepad.exe")) as string;

    await Assert.That(stored).IsEqualTo(@"..\notepad.exe");
  }

  [Test]
  [RunOn(OS.Windows)]
  public async Task ConvertToProvider_SiblingOfTheBasePath_IsNotTruncated()
  {
    var converter =
      new FileInfoToStringConverter('\\', new DirectoryInfo(@"C:\Windows\System"), MappingHints);

    var stored = converter.ConvertToProvider(new FileInfo(@"C:\Windows\System32\hosts")) as string;

    await Assert.That(stored).IsEqualTo(@"..\System32\hosts");
  }
}
