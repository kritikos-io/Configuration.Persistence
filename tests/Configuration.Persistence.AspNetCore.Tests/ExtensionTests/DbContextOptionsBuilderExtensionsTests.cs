#pragma warning disable SA1402 // File may only contain a single type

namespace Kritikos.Configuration.Persistence.AspNetCore.Tests.ExtensionTests;

using System;
using System.Threading.Tasks;

using Kritikos.Configuration.Persistence.AspNetCore.Extensions;
using Kritikos.Samples.CityCensus;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

public class DbContextOptionsBuilderExtensionsTests
{
  [Test]
  public async Task EnableCommonOptions_NullEnvironment_ThrowsArgumentNullException()
    => await Assert.That(() => new DbContextOptionsBuilder().EnableCommonOptions(null!))
      .Throws<ArgumentNullException>();

  [Test]
  public async Task EnableCommonOptions_NullEnvironmentOnTypedBuilder_ThrowsArgumentNullException()
    => await Assert.That(
        () => new DbContextOptionsBuilder<CityCensusTrailDbContext>().EnableCommonOptions(null!))
      .Throws<ArgumentNullException>();

  [Test]
  [Arguments("Development", true)]
  [Arguments("Staging", false)]
  [Arguments("Production", false)]
  public async Task EnableCommonOptions_DiagnosticOptions_FollowTheEnvironment(
    string environmentName,
    bool expected)
  {
    var builder = new DbContextOptionsBuilder();

    builder.EnableCommonOptions(new StubHostEnvironment(environmentName));

    var options = Core(builder);
    await Assert.That(options.DetailedErrorsEnabled).IsEqualTo(expected);
    await Assert.That(options.IsSensitiveDataLoggingEnabled).IsEqualTo(expected);
  }

  [Test]
  [Arguments("Development", true)]
  [Arguments("Production", false)]
  public async Task EnableCommonOptions_TypedBuilder_FollowsTheEnvironment(string environmentName, bool expected)
  {
    var builder = new DbContextOptionsBuilder<CityCensusTrailDbContext>();

    var returned = builder.EnableCommonOptions(new StubHostEnvironment(environmentName));

    await Assert.That(returned).IsSameReferenceAs(builder);
    var options = Core(builder);
    await Assert.That(options.DetailedErrorsEnabled).IsEqualTo(expected);
    await Assert.That(options.IsSensitiveDataLoggingEnabled).IsEqualTo(expected);
  }

  private static CoreOptionsExtension Core(DbContextOptionsBuilder builder)
    => builder.Options.FindExtension<CoreOptionsExtension>()
       ?? throw new InvalidOperationException("The builder carries no core options.");
}

internal sealed class StubHostEnvironment(string environmentName) : IHostEnvironment
{
  public string EnvironmentName { get; set; } = environmentName;

  public string ApplicationName { get; set; } = nameof(DbContextOptionsBuilderExtensionsTests);

  public string ContentRootPath { get; set; } = AppContext.BaseDirectory;

  public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
}
