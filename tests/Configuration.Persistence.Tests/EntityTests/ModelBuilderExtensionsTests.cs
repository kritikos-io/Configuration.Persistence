namespace Kritikos.Configuration.Persistence.Tests.EntityTests;

using System;
using System.Threading.Tasks;

using Kritikos.Configuration.Persistence.Contracts;
using Kritikos.Configuration.Persistence.Contracts.Behavioral;
using Kritikos.Configuration.Persistence.Extensions;
using Kritikos.Configuration.Persistence.Tests.EntityTests.Concurrency;

using Microsoft.EntityFrameworkCore;

public class ModelBuilderExtensionsTests
{
  [Test]
  public async Task ApplyConcurrencyTokens_SqlServerEntity_MapsRowVersionAsConcurrencyToken()
  {
    await using var ctx = new ConcurrencyDbContext();

    var entity = ctx.Model.FindEntityType(typeof(SqlServerConcurrentEntity));
    var rowVersion = entity?.FindProperty(nameof(ISqlServerConcurrent.RowVersion));

    await Assert.That(rowVersion).IsNotNull();
    await Assert.That(rowVersion?.IsConcurrencyToken).IsTrue();
  }

  [Test]
  public async Task ApplyConcurrencyTokens_SqlServerEntity_DoesNotCreateShadowProperty()
  {
    await using var ctx = new ConcurrencyDbContext();

    var entity = ctx.Model.FindEntityType(typeof(SqlServerConcurrentEntity));
    var shadow = entity?.FindProperty(nameof(ISqlServerConcurrent));

    await Assert.That(shadow).IsNull();
  }

  [Test]
  public async Task ApplyConcurrencyTokens_PostgreSqlEntity_MapsRowVersionAsConcurrencyToken()
  {
    await using var ctx = new ConcurrencyDbContext();

    var entity = ctx.Model.FindEntityType(typeof(PostgreSqlConcurrentEntity));
    var rowVersion = entity?.FindProperty(nameof(IPostgreSqlConcurrent.RowVersion));

    await Assert.That(rowVersion).IsNotNull();
    await Assert.That(rowVersion?.IsConcurrencyToken).IsTrue();
  }

  [Test]
  public async Task ApplyConcurrencyTokens_NullModelBuilder_ThrowsArgumentNullException()
    => await Assert.That(() => ((ModelBuilder)null!).ApplyConcurrencyTokens())
      .Throws<ArgumentNullException>();

  [Test]
  public async Task ApplySoftDeletableFilters_NullModelBuilder_ThrowsArgumentNullException()
    => await Assert.That(() => ((ModelBuilder)null!).ApplySoftDeletableFilters())
      .Throws<ArgumentNullException>();

  [Test]
  public async Task EntitiesImplementing_NullModelBuilder_ThrowsArgumentNullException()
    => await Assert.That(() => ((ModelBuilder)null!).EntitiesImplementing<ISoftDeletable>(_ => { }))
      .Throws<ArgumentNullException>();

  [Test]
  public async Task EntitiesImplementing_NullBuildAction_ThrowsArgumentNullException()
    => await Assert.That(() => new ModelBuilder().EntitiesImplementing<ISoftDeletable>(null!))
      .Throws<ArgumentNullException>();

  [Test]
  public async Task EntitiesOfType_NullModelBuilder_ThrowsArgumentNullException()
    => await Assert.That(() => ((ModelBuilder)null!).EntitiesOfType<SqlServerConcurrentEntity>(_ => { }))
      .Throws<ArgumentNullException>();

  [Test]
  public async Task EntitiesOfType_NullBuildAction_ThrowsArgumentNullException()
    => await Assert.That(() => new ModelBuilder().EntitiesOfType<SqlServerConcurrentEntity>(null!))
      .Throws<ArgumentNullException>();
}
