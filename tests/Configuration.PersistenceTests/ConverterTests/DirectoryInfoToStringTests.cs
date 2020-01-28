namespace Kritikos.Configuration.PersistenceTests.ConverterTests
{
	using System;
	using System.Diagnostics.CodeAnalysis;
	using System.IO;

	using Kritikos.Configuration.Persistence.Converters;

	using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

	using Xunit;

	[ExcludeFromCodeCoverage]
	public class DirectoryInfoToStringTests
	{
		private static readonly string AssemblyName =
			typeof(DirectoryInfoToStringConverter).Assembly.GetName().Name ?? string.Empty;

		// Hack to  make path name work during Live Unit Testing in Visual Studio
		// since it runs on a different directory
		private static readonly string BaseFolder = Directory.GetCurrentDirectory()
			.Substring(0,
				Directory.GetCurrentDirectory()
					.IndexOf(AssemblyName.Replace('.', Path.DirectorySeparatorChar),
						StringComparison.InvariantCulture) + AssemblyName.Length);

		private const string ActualPath = "src/Configuration.Persistence";
		
		private static readonly ConverterMappingHints Hints = new ConverterMappingHints(unicode: true);

		[Fact]
		public void WithBaseFolder()
		{
			var converter =
				new DirectoryInfoToStringConverter($"{BaseFolder}{Path.DirectorySeparatorChar}", '/', Hints);
			var dirToString = converter.ConvertToProviderExpression.Compile();
			var stringToDir = converter.ConvertFromProviderExpression.Compile();

			var dir = stringToDir(ActualPath);
			Assert.True(dir.Exists);

			var reverse = dirToString(dir);
			Assert.Equal(ActualPath, reverse);

			var invalidDir = new DirectoryInfo(ActualPath.Replace('/','\\'));
			var invalidStr = dirToString(invalidDir);
			Assert.Equal(ActualPath,invalidStr);
		}

		[Fact]
		public void WithoutBaseFolder()
		{
			var converter = new DirectoryInfoToStringConverter(string.Empty,Path.DirectorySeparatorChar,Hints);
			var dirToString = converter.ConvertToProviderExpression.Compile();
			var stringToDir = converter.ConvertFromProviderExpression.Compile();

			var dir = stringToDir(ActualPath);
			Assert.False(dir.Exists);
			var sanitized = dirToString(dir).Replace(Path.DirectorySeparatorChar, '/');
			Assert.Equal(ActualPath,sanitized);
		}
	}
}