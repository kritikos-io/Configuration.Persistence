namespace Kritikos.Configuration.PersistenceTests.ConverterTests
{
	using System;
	using System.Collections.Generic;
	using System.IO;

	using Kritikos.Configuration.Persistence.Converters;

	using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

	using Xunit;

	public class FileInfoToStringTests
	{
		private static readonly string AssemblyName =
			typeof(FileInfoToStringConverter).Assembly.GetName().Name ?? string.Empty;

		// Hack to  make path name work during Live Unit Testing in Visual Studio
		// since it runs on a different directory
		private static readonly string BaseFolder = Directory.GetCurrentDirectory()
														.Substring(0,
															Directory.GetCurrentDirectory()
																.IndexOf(AssemblyName,
																	StringComparison.InvariantCulture)
															+ AssemblyName.Length)
													+ Path.DirectorySeparatorChar;

		private const string ActualFile = "src/Configuration.Persistence/Configuration.Persistence.csproj";

		private static readonly ConverterMappingHints Hints = new ConverterMappingHints(unicode: true);

		[Fact]
		public void WithoutBaseFolder()
		{
			var converter =
				new FileInfoToStringConverter(Hints, BaseFolder, "base", '/', new Dictionary<string, string>());

			var fileToString = converter.ConvertToProviderExpression.Compile();
			var stringToFile = converter.ConvertFromProviderExpression.Compile();

			var file = stringToFile(ActualFile);
			Assert.True(file.Exists);

			var reverse = fileToString(file);
			Assert.Equal(ActualFile, reverse);
		}

		[Fact]
		public void WithBaseFolder()
		{
			var converter =
				new FileInfoToStringConverter(Hints, BaseFolder, "noBase", '/', new Dictionary<string, string>());

			var dirToString = converter.ConvertToProviderExpression.Compile();
			var stringToDir = converter.ConvertFromProviderExpression.Compile();

			var dir = stringToDir(ActualFile);

			Assert.True(dir.Exists);

			var reverse = dirToString(dir);
			Assert.EndsWith(ActualFile, reverse, StringComparison.InvariantCulture);
		}

		[Fact]
		public void WithSubstitute()
		{
			var substitutions = new Dictionary<string, string> { { @"/mnt/storage/Media/", @"Z:\" }, };
			var converter = new FileInfoToStringConverter(Hints, string.Empty, "substitutes", '/', substitutions);

			var fileToString = converter.ConvertToProviderExpression.Compile();
			var stringToFile = converter.ConvertFromProviderExpression.Compile();

			const string episode = @"/mnt/storage/Media/Big Buck Bunny.avi";

			var file = stringToFile(episode);
			Assert.False(file.Exists);

			var reverse = fileToString(file);
			Assert.Equal(episode, reverse);
		}
	}
}
