namespace Kritikos.Configuration.Persistence.Tests.ExtensionTests;

using System;

using Kritikos.Configuration.Persistence.Extensions;

using Microsoft.EntityFrameworkCore;

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
}
