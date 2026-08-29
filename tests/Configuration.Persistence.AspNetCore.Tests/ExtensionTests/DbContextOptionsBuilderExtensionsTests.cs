namespace Kritikos.Configuration.Persistence.AspNetCore.Tests.ExtensionTests;

using System;
using System.Threading.Tasks;

using Kritikos.Configuration.Persistence.AspNetCore.Extensions;
using Kritikos.Samples.CityCensus;

using Microsoft.EntityFrameworkCore;

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
}
