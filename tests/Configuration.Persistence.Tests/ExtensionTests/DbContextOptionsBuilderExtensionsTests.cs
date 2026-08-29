namespace Kritikos.Configuration.Persistence.Tests.ExtensionTests;

using System;

using Kritikos.Configuration.Persistence.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;

public class DbContextOptionsBuilderExtensionsTests
{
  [Test]
  public async Task EnableCommonOptions_NullBuilder_ThrowsArgumentNullException()
    => await Assert.That(() => ((DbContextOptionsBuilder)null!).EnableCommonOptions(true))
      .Throws<ArgumentNullException>();

  [Test]
  public async Task EnableCommonOptions_NullTypedBuilder_ThrowsArgumentNullException()
    => await Assert.That(() => ((DbContextOptionsBuilder<DbContext>)null!).EnableCommonOptions(true))
      .Throws<ArgumentNullException>();

  [Test]
  public async Task EnableCommonOptions_TypedBuilder_ReturnsTheSameInstance()
  {
    var builder = new DbContextOptionsBuilder<DbContext>();

    await Assert.That(builder.EnableCommonOptions(isDevelopment: true)).IsSameReferenceAs(builder);
  }

  [Test]
  public async Task EnableCommonOptions_UntypedBuilder_ReturnsTheSameInstance()
  {
    var builder = new DbContextOptionsBuilder();

    await Assert.That(builder.EnableCommonOptions(isDevelopment: false)).IsSameReferenceAs(builder);
  }

  [Test]
  [Arguments(true)]
  [Arguments(false)]
  public async Task EnableCommonOptions_DiagnosticOptions_FollowTheDevelopmentFlag(bool isDevelopment)
  {
    var builder = new DbContextOptionsBuilder();

    builder.EnableCommonOptions(isDevelopment);

    var options = Core(builder);
    await Assert.That(options.DetailedErrorsEnabled).IsEqualTo(isDevelopment);
    await Assert.That(options.IsSensitiveDataLoggingEnabled).IsEqualTo(isDevelopment);
  }

  [Test]
  [Arguments(true)]
  [Arguments(false)]
  public async Task EnableCommonOptions_TypedBuilder_FollowsTheDevelopmentFlag(bool isDevelopment)
  {
    var builder = new DbContextOptionsBuilder<DbContext>();

    builder.EnableCommonOptions(isDevelopment);

    var options = Core(builder);
    await Assert.That(options.DetailedErrorsEnabled).IsEqualTo(isDevelopment);
    await Assert.That(options.IsSensitiveDataLoggingEnabled).IsEqualTo(isDevelopment);
  }

  [Test]
  [Arguments(true)]
  [Arguments(false)]
  public async Task EnableCommonOptions_CascadingDeletes_ThrowRegardlessOfTheDevelopmentFlag(bool isDevelopment)
  {
    var builder = new DbContextOptionsBuilder();

    builder.EnableCommonOptions(isDevelopment);

    var warnings = Core(builder).WarningsConfiguration;
    await Assert.That(warnings.GetBehavior(CoreEventId.CascadeDelete)).IsEqualTo(WarningBehavior.Throw);
    await Assert.That(warnings.GetBehavior(CoreEventId.CascadeDeleteOrphan)).IsEqualTo(WarningBehavior.Throw);
    await Assert.That(warnings.GetBehavior(CoreEventId.SensitiveDataLoggingEnabledWarning))
      .IsEqualTo(WarningBehavior.Ignore);
  }

  private static CoreOptionsExtension Core(DbContextOptionsBuilder builder)
    => builder.Options.FindExtension<CoreOptionsExtension>()
       ?? throw new InvalidOperationException("The builder carries no core options.");
}
