namespace Kritikos.Configuration.Persistence.Tests.EntityTests;

using System;
using System.Linq;
using System.Threading.Tasks;

using Kritikos.Configuration.Persistence.Contracts;
using Kritikos.Configuration.Persistence.Contracts.Behavioral;
using Kritikos.Configuration.Persistence.Extensions;
using Kritikos.Configuration.Persistence.Tests.EntityTests.Concurrency;
using Kritikos.Configuration.Persistence.Tests.EntityTests.Ordering;
using Kritikos.Configuration.Persistence.Tests.EntityTests.SoftDelete;

using Microsoft.Data.Sqlite;
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
  public async Task ApplySoftDeletableFilters_SoftDeletableEntity_DefaultsIsDeletedToFalse()
  {
    await using var ctx = new SoftDeleteDbContext();

    var entity = ctx.Model.FindEntityType(typeof(SoftDeletableEntity));
    var isDeleted = entity?.FindProperty(nameof(ISoftDeletable.IsDeleted));

    await Assert.That(isDeleted).IsNotNull();
    await Assert.That((bool)isDeleted!.GetDefaultValue()!).IsFalse();
  }

  [Test]
  public async Task ApplySoftDeletableFilters_DeletedEntity_IsExcludedFromQueries(
    CancellationToken cancellationToken)
  {
    await using var connection = new SqliteConnection("DataSource=softdelete_model;mode=memory;cache=shared");
    await connection.OpenAsync(cancellationToken);

    await using var ctx = new SoftDeleteDbContext();
    await ctx.Database.EnsureCreatedAsync(cancellationToken);

    ctx.Entities.AddRange(
      new SoftDeletableEntity { Name = "visible" },
      new SoftDeletableEntity { Name = "hidden", IsDeleted = true });
    await ctx.SaveChangesAsync(cancellationToken);
    ctx.ChangeTracker.Clear();

    var visible = await ctx.Entities.ToListAsync(cancellationToken);
    var all = await ctx.Entities.IgnoreQueryFilters().ToListAsync(cancellationToken);

    await Assert.That(visible.Select(x => x.Name).ToList()).IsEquivalentTo(["visible"]);
    await Assert.That(all.Count).IsEqualTo(2);
  }

  [Test]
  public async Task ApplySoftDeletableFilters_NullModelBuilder_ThrowsArgumentNullException()
    => await Assert.That(() => ((ModelBuilder)null!).ApplySoftDeletableFilters())
      .Throws<ArgumentNullException>();

  [Test]
  public async Task EntitiesImplementing_NullModelBuilder_ThrowsArgumentNullException()
    => await Assert.That(() => ((ModelBuilder)null!).EntitiesImplementing<ISoftDeletable>(_ => { }))
      .Throws<ArgumentNullException>();

  [Test]
  public async Task EntitiesImplementing_EveryImplementorOfTheInterface_IsConfiguredIndividually()
  {
    await using var ctx = new OrderingDbContext();

    var labelled = ctx.Model.FindEntityType(typeof(LabelledEntity))?.FindProperty(nameof(ILabelled.Label));
    var both = ctx.Model.FindEntityType(typeof(LabelledAndNumberedEntity))
      ?.FindDeclaredProperty(nameof(ILabelled.Label));

    await Assert.That(labelled?.GetDefaultValue()).IsEqualTo(OrderingDbContext.LabelDefault);
    await Assert.That(both?.GetDefaultValue()).IsEqualTo(OrderingDbContext.LabelDefault);
  }

  [Test]
  public async Task EntitiesImplementing_TypeNotImplementingTheInterface_IsLeftAlone()
  {
    await using var ctx = new OrderingDbContext();

    var label = ctx.Model.FindEntityType(typeof(OnlyNumberedEntity))?.FindProperty(nameof(ILabelled.Label));

    await Assert.That(label).IsNull();
  }

  [Test]
  public async Task EntitiesOfType_BaseClass_IsConfiguredOnceAndInheritedByDerivedTypes()
  {
    await using var ctx = new OrderingDbContext();

    var declaredOnBase = ctx.Model.FindEntityType(typeof(NumberedEntity))
      ?.FindDeclaredProperty(nameof(NumberedEntity.Number));
    var declaredOnDerived = ctx.Model.FindEntityType(typeof(OnlyNumberedEntity))
      ?.FindDeclaredProperty(nameof(NumberedEntity.Number));

    await Assert.That(declaredOnBase?.GetDefaultValue()).IsEqualTo(OrderingDbContext.NumberDefault);
    await Assert.That(declaredOnDerived).IsNull();
  }

  [Test]
  public async Task EntitiesOfType_TypeOutsideTheHierarchy_IsLeftAlone()
  {
    await using var ctx = new OrderingDbContext();

    var number = ctx.Model.FindEntityType(typeof(LabelledEntity))?.FindProperty(nameof(NumberedEntity.Number));

    await Assert.That(number).IsNull();
  }

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
