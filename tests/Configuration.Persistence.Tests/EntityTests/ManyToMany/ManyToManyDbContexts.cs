#pragma warning disable SA1402 // File may only contain a single type
namespace Kritikos.Configuration.Persistence.Tests.EntityTests.ManyToMany;

using System;

using Kritikos.Configuration.Persistence.Extensions;

using Microsoft.EntityFrameworkCore;

public abstract class ManyToManyDbContext : DbContext
{
  protected abstract string DatabaseName { get; }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    ArgumentNullException.ThrowIfNull(optionsBuilder);

    if (!optionsBuilder.IsConfigured)
    {
      optionsBuilder.UseSqlite($"DataSource={DatabaseName};mode=memory");
    }

    base.OnConfiguring(optionsBuilder);
  }
}

public class BothNavigationsDbContext : ManyToManyDbContext
{
  public DbSet<AuthorBook> AuthorBooks => Set<AuthorBook>();

  protected override string DatabaseName => "m2m_both";

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    ArgumentNullException.ThrowIfNull(modelBuilder);
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<AuthorBook>()
      .ManyToManyWithJoinEntity(x => x.Author, x => x.Book, a => a.AuthorBooks, b => b.AuthorBooks);
  }
}

public class RightNavigationOnlyDbContext : ManyToManyDbContext
{
  public DbSet<AuthorBook> AuthorBooks => Set<AuthorBook>();

  protected override string DatabaseName => "m2m_right";

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    ArgumentNullException.ThrowIfNull(modelBuilder);
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<AuthorBook>()
      .ManyToManyWithJoinEntity(x => x.Author, x => x.Book, reverseRight: b => b.AuthorBooks);
  }
}

public class CustomForeignKeysDbContext : ManyToManyDbContext
{
  public DbSet<AuthorBook> AuthorBooks => Set<AuthorBook>();

  protected override string DatabaseName => "m2m_custom";

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    ArgumentNullException.ThrowIfNull(modelBuilder);
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<AuthorBook>()
      .ManyToManyWithJoinEntity(
        x => x.Author,
        x => x.Book,
        a => a.AuthorBooks,
        b => b.AuthorBooks,
        "WriterId",
        "TitleId");
  }
}

public class SkipNavigationDbContext : ManyToManyDbContext
{
  public DbSet<Painter> Painters => Set<Painter>();

  protected override string DatabaseName => "m2m_skip";

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    ArgumentNullException.ThrowIfNull(modelBuilder);
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Painter>()
      .ManyToManyWithSkipNavigation<PainterGallery, Painter, Gallery>(x => x.Galleries, x => x.Painters);
  }
}
