namespace Kritikos.Configuration.PersistenceTests.EntityTests.Concurrency;

using System.Threading.Tasks;

using Kritikos.Configuration.Persistence.Contracts;

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
}
