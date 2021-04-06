namespace Kritikos.Configuration.PersistenceTests.ConverterTests
{
  using System.IO;

  using FluentAssertions;

  using Kritikos.Configuration.Persistence.Converters;

  using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

  using Xunit;

  public class DirectoryInfoToStringTests
  {
    private const string WindowsBase = @"C:\Windows\System32";
    private const string WindowsRelative = @"drivers\etc";

    private const string WindowsPath = @"C:\Windows\System32\drivers\etc";

    private const string LinuxBase = @"/srv/http";
    private const string LinuxRelative = @"root";
    private const string LinuxPath = @"/srv/http/root";

    private static readonly ConverterMappingHints MappingHints = new(unicode: true);

    [Fact]
    public void Relative_path_windows()
    {
      var converter =
        new DirectoryInfoToStringConverter('\\', new DirectoryInfo(WindowsBase), MappingHints);

      var file = converter.ConvertFromProvider(WindowsRelative) as DirectoryInfo;
      var foo = converter.ConvertToProvider(file) as string;

      file!.FullName.Should().Be(WindowsPath);
      foo.Should().Be(WindowsRelative);
    }

    [Fact]
    public void Absolute_path_windows()
    {
      var converter =
        new DirectoryInfoToStringConverter('\\', mappingHints: MappingHints);

      var file = converter.ConvertFromProvider(WindowsPath) as DirectoryInfo;
      var foo = converter.ConvertToProvider(file) as string;

      file!.FullName.Should().Be(WindowsPath);
      foo.Should().Be(WindowsPath);
    }

    [Fact]
    public void Relative_path_linux()
    {
      var converter =
        new DirectoryInfoToStringConverter('/', new DirectoryInfo(LinuxBase),
          mappingHints: MappingHints);

      var file = converter.ConvertFromProvider(LinuxRelative) as DirectoryInfo;
      var foo = converter.ConvertToProvider(file) as string;

      foo.Should().Be(LinuxRelative);
    }

    [Fact]
    public void Absolute_path_linux()
    {
      var converter =
        new DirectoryInfoToStringConverter('/', mappingHints: MappingHints);

      var file = converter.ConvertFromProvider(LinuxPath) as DirectoryInfo;
      var foo = converter.ConvertToProvider(file) as string;

      foo.Should().Be(LinuxPath);
    }
  }
}
