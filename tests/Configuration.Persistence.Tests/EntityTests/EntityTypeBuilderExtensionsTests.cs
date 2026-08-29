namespace Kritikos.Configuration.Persistence.Tests.EntityTests;

using System;
using System.Linq;
using System.Threading.Tasks;

using Kritikos.Configuration.Persistence.Extensions;
using Kritikos.Configuration.Persistence.Tests.EntityTests.ManyToMany;

public class EntityTypeBuilderExtensionsTests
{
  [Test]
  public async Task ManyToManyWithJoinEntity_BothReverseNavigations_ConfiguresCollectionOnEachSide()
  {
    await using var ctx = new BothNavigationsDbContext();

    var left = ctx.Model.FindEntityType(typeof(Author))?.FindNavigation(nameof(Author.AuthorBooks));
    var right = ctx.Model.FindEntityType(typeof(Book))?.FindNavigation(nameof(Book.AuthorBooks));

    await Assert.That(left).IsNotNull();
    await Assert.That(left!.ForeignKey.Properties[0].Name).IsEqualTo("AuthorId");
    await Assert.That(right).IsNotNull();
    await Assert.That(right!.ForeignKey.Properties[0].Name).IsEqualTo("BookId");
  }

  [Test]
  public async Task ManyToManyWithJoinEntity_OnlyRightReverseNavigation_StillConfiguresRightCollection()
  {
    await using var ctx = new RightNavigationOnlyDbContext();

    var right = ctx.Model.FindEntityType(typeof(Book))?.FindNavigation(nameof(Book.AuthorBooks));

    await Assert.That(right).IsNotNull();
    await Assert.That(right!.ForeignKey.Properties[0].Name).IsEqualTo("BookId");
  }

  [Test]
  public async Task ManyToManyWithJoinEntity_DefaultForeignKeyNames_UsesCompositePrimaryKey()
  {
    await using var ctx = new BothNavigationsDbContext();

    var key = ctx.Model.FindEntityType(typeof(AuthorBook))?.FindPrimaryKey();

    await Assert.That(key).IsNotNull();
    await Assert.That(key!.Properties.Select(x => x.Name).ToList()).IsEquivalentTo(["AuthorId", "BookId"]);
  }

  [Test]
  public async Task ManyToManyWithJoinEntity_CustomForeignKeyNames_UsesThemForTheCompositeKey()
  {
    await using var ctx = new CustomForeignKeysDbContext();

    var key = ctx.Model.FindEntityType(typeof(AuthorBook))?.FindPrimaryKey();

    await Assert.That(key).IsNotNull();
    await Assert.That(key!.Properties.Select(x => x.Name).ToList()).IsEquivalentTo(["WriterId", "TitleId"]);
  }

  [Test]
  public async Task ManyToManyWithSkipNavigation_ExplicitJoinEntity_BacksBothSkipNavigations()
  {
    await using var ctx = new SkipNavigationDbContext();

    var left = ctx.Model.FindEntityType(typeof(Painter))?.FindSkipNavigation(nameof(Painter.Galleries));
    var right = ctx.Model.FindEntityType(typeof(Gallery))?.FindSkipNavigation(nameof(Gallery.Painters));

    await Assert.That(left).IsNotNull();
    await Assert.That(left!.JoinEntityType.ClrType).IsEqualTo(typeof(PainterGallery));
    await Assert.That(right).IsNotNull();
    await Assert.That(right!.JoinEntityType.ClrType).IsEqualTo(typeof(PainterGallery));
  }

  [Test]
  public async Task ManyToManyWithJoinEntity_NullEntityTypeBuilder_ThrowsArgumentNullException()
    => await Assert.That(
        () => EntityTypeBuilderExtensions.ManyToManyWithJoinEntity<AuthorBook, Author, Book>(
          null!,
          x => x.Author,
          x => x.Book))
      .Throws<ArgumentNullException>();

  [Test]
  public async Task ManyToManyWithSkipNavigation_NullEntityTypeBuilder_ThrowsArgumentNullException()
    => await Assert.That(
        () => EntityTypeBuilderExtensions.ManyToManyWithSkipNavigation<PainterGallery, Painter, Gallery>(
          null!,
          x => x.Galleries))
      .Throws<ArgumentNullException>();
}
