namespace Kritikos.Configuration.PersistenceTests.ConverterTests
{
  using System.IO;

  using FluentAssertions;

  using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

  using Xunit;

  public class FileInfoToStringTests
  {
    private const string WindowsBase = @"C:\Windows\System32";
    private const string WindowsRelative = @"drivers\etc\hosts";

    private const string WindowsPath = @"C:\Windows\System32\drivers\etc\hosts";

    private const string LinuxBase = @"/srv/http";
    private const string LinuxRelative = @"root/index.html";
    private const string LinuxPath = @"/srv/http/root/index.html";

    private static readonly ConverterMappingHints MappingHints = new(unicode: true);

    [Fact]
    public void Relative_path_windows()
    {
      var converter =
        new Persistence.Converters.FileInfoToStringConverter('\\', new DirectoryInfo(WindowsBase), MappingHints);

      var file = converter.ConvertFromProvider(WindowsRelative) as FileInfo;
      var foo = converter.ConvertToProvider(file) as string;

      file.FullName.Should().Be(WindowsPath);
      foo.Should().Be(WindowsRelative);
    }

    [Fact]
    public void Absolute_path_windows()
    {
      var converter =
        new Persistence.Converters.FileInfoToStringConverter('\\', mappingHints: MappingHints);

      var file = converter.ConvertFromProvider(WindowsPath) as FileInfo;
      var foo = converter.ConvertToProvider(file) as string;

      file.FullName.Should().Be(WindowsPath);
      foo.Should().Be(WindowsPath);
    }

    [Fact]
    public void Relative_path_linux()
    {
      var converter =
        new Persistence.Converters.FileInfoToStringConverter('/', new DirectoryInfo(LinuxBase),
          mappingHints: MappingHints);

      var file = converter.ConvertFromProvider(LinuxRelative) as FileInfo;
      var foo = converter.ConvertToProvider(file) as string;

      foo.Should().Be(LinuxRelative);
    }

    [Fact]
    public void Absolute_path_linux()
    {
      var converter =
        new Persistence.Converters.FileInfoToStringConverter('/', mappingHints: MappingHints);

      var file = converter.ConvertFromProvider(LinuxPath) as FileInfo;
      var foo = converter.ConvertToProvider(file) as string;

      foo.Should().Be(LinuxPath);
    }
  }
}