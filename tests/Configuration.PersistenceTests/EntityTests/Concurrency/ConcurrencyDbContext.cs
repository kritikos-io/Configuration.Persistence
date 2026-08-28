namespace Kritikos.Configuration.PersistenceTests.EntityTests.Concurrency;

using System;

using Kritikos.Configuration.Persistence.Extensions;

using Microsoft.EntityFrameworkCore;

public class ConcurrencyDbContext : DbContext
{
  public DbSet<SqlServerConcurrentEntity> SqlServerEntities => Set<SqlServerConcurrentEntity>();

  public DbSet<PostgreSqlConcurrentEntity> PostgreSqlEntities => Set<PostgreSqlConcurrentEntity>();

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    ArgumentNullException.ThrowIfNull(optionsBuilder);

    if (!optionsBuilder.IsConfigured)
    {
      optionsBuilder.UseSqlite("DataSource=concurrency_model;mode=memory");
    }

    base.OnConfiguring(optionsBuilder);
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    ArgumentNullException.ThrowIfNull(modelBuilder);
    base.OnModelCreating(modelBuilder);

    modelBuilder.ApplyConcurrencyTokens();
  }
}
