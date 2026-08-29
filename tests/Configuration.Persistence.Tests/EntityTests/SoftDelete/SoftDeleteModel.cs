#pragma warning disable SA1402 // File may only contain a single type
namespace Kritikos.Configuration.Persistence.Tests.EntityTests.SoftDelete;

using System;

using Kritikos.Configuration.Persistence.Contracts.Behavioral;
using Kritikos.Configuration.Persistence.Extensions;

using Microsoft.EntityFrameworkCore;

public class SoftDeletableEntity : IEntity<long>, ISoftDeletable
{
  public long Id { get; set; }

  public string Name { get; set; } = string.Empty;

  public bool IsDeleted { get; set; }

  public DateTime? DeletedAt { get; set; }
}

public class SoftDeletableRoot : IEntity<long>, ISoftDeletable
{
  public long Id { get; set; }

  public string Name { get; set; } = string.Empty;

  public bool IsDeleted { get; set; }

  public DateTime? DeletedAt { get; set; }
}

public class SoftDeletableLeaf : SoftDeletableRoot
{
  public string Extra { get; set; } = string.Empty;
}

public class SoftDeleteDbContext : DbContext
{
  public DbSet<SoftDeletableEntity> Entities => Set<SoftDeletableEntity>();

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    ArgumentNullException.ThrowIfNull(optionsBuilder);

    if (!optionsBuilder.IsConfigured)
    {
      optionsBuilder.UseSqlite("DataSource=softdelete_model;mode=memory;cache=shared");
    }

    base.OnConfiguring(optionsBuilder);
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    ArgumentNullException.ThrowIfNull(modelBuilder);
    base.OnModelCreating(modelBuilder);

    modelBuilder.ApplySoftDeletableFilters();
  }
}

public class SoftDeleteHierarchyDbContext : DbContext
{
  public DbSet<SoftDeletableRoot> Entities => Set<SoftDeletableRoot>();

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    ArgumentNullException.ThrowIfNull(optionsBuilder);

    if (!optionsBuilder.IsConfigured)
    {
      optionsBuilder.UseSqlite("DataSource=softdelete_hierarchy;mode=memory;cache=shared");
    }

    base.OnConfiguring(optionsBuilder);
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    ArgumentNullException.ThrowIfNull(modelBuilder);
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<SoftDeletableLeaf>();
    modelBuilder.ApplySoftDeletableFilters();
  }
}
