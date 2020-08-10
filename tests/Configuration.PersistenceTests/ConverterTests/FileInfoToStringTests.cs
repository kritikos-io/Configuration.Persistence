namespace Kritikos.Configuration.PersistenceTests.ConverterTests
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;

	using Kritikos.Configuration.Persistence.Converters;

	using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

	using Xunit;

	public class FileInfoToStringTests
	{
		private static readonly string AssemblyName =
			typeof(FileInfoToStringConverter).Assembly.GetName().Name ?? string.Empty;

		// Hack to  make path name work during Live Unit Testing in Visual Studio
		// since it runs on a different directory
		//private static readonly string BaseFolder = Directory.GetCurrentDirectory()
		//												.Substring(0,
		//													Directory.GetCurrentDirectory()
		//														.IndexOf(AssemblyName,
		//															StringComparison.InvariantCulture)
		//													+ AssemblyName.Length)
		//											+ Path.DirectorySeparatorChar;

		private static readonly string CurrentDirectory = Directory.GetCurrentDirectory();

		private static readonly string BaseFolder =
			CurrentDirectory.Substring(0, CurrentDirectory.IndexOf("tests", StringComparison.InvariantCulture));

		private static readonly string AppName = typeof(FileInfoToStringConverter).Assembly.GetName().Name;

		private const string ActualFile = "src/Configuration.Persistence/Configuration.Persistence.csproj";

		private static readonly ConverterMappingHints Hints = new ConverterMappingHints(unicode: true);

		[Fact]
		public void WithoutBaseFolder()
		{
			var converter =
				new FileInfoToStringConverter(Hints, "base", BaseFolder, '/', new Dictionary<string, string>());

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
				new FileInfoToStringConverter(Hints, "noBase", BaseFolder, '/', new Dictionary<string, string>());

			var dirToString = converter.ConvertToProviderExpression.Compile();
			var stringToDir = converter.ConvertFromProviderExpression.Compile();

			var dir = stringToDir(ActualFile);

			Assert.True(dir.Exists, $"File {dir.FullName} did not exist!");

			var reverse = dirToString(dir);
			Assert.EndsWith(ActualFile, reverse, StringComparison.InvariantCulture);
		}

		[Fact]
		public void WithSubstitute()
		{
			var currentDir = Path.Combine(Directory.GetCurrentDirectory(), Path.DirectorySeparatorChar.ToString());

			var substitutions = new Dictionary<string, string> { { @"/mnt/storage/Media/", @"Z:\" }, };
			var converter = new FileInfoToStringConverter(Hints, "substitutes", currentDir, '/',
				substitutions);

			var fileToString = converter.ConvertToProviderExpression.Compile();
			var stringToFile = converter.ConvertFromProviderExpression.Compile();

			const string episode = @"/mnt/storage/Media/Big Buck Bunny.avi";

			var file = stringToFile(episode);

			Assert.StartsWith(substitutions.First().Value,
				file.FullName.Replace(currentDir, string.Empty));

			var reverse = fileToString(file);
			Assert.Equal(episode, reverse);
		}
	}
}
