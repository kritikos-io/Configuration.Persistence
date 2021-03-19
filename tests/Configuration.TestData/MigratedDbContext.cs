#nullable disable
using System.Diagnostics.CodeAnalysis;

[assembly: ExcludeFromCodeCoverage]

namespace Kritikos.Configuration.TestData
{
  using System;

  using Kritikos.Configuration.Persistence.Extensions;
  using Kritikos.Configuration.TestData.Base;
  using Kritikos.Configuration.TestData.Model;

  using Microsoft.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore.ValueGeneration;

  [ExcludeFromCodeCoverage]
  public class MigratedDbContext : DbContext
  {
    public MigratedDbContext()
    {
    }

    public MigratedDbContext(DbContextOptions<MigratedDbContext> options)
      : base(options)
    {
    }

    public DbSet<AuditRecord> Records { get; set; }

    public DbSet<Person> People { get; set; }

    public DbSet<Order> Orders { get; set; }

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      if (optionsBuilder.IsConfigured)
      {
        base.OnConfiguring(optionsBuilder);
        return;
      }

      optionsBuilder.UseSqlite("DataSource=transient;mode=memory;cache=shared")
        .EnableCommonOptions(true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
      modelBuilder.EntitiesOfType<TestEntity<long>>(entity =>
      {
        entity.HasKey(entity => entity.Id);
        entity.HasBaseType((Type)null);
      });

      modelBuilder.EntitiesOfType<IObfuscated>(entity =>
        entity.Property(typeof(Guid), nameof(IObfuscated.Order))
          .HasValueGenerator((p, t) => new GuidValueGenerator()));

      modelBuilder.EntitiesOfType<OrderedEntity<long>>(entity =>
        entity.Property(e => e.Order).HasValueGenerator((p, t) => new GuidValueGenerator()));

      modelBuilder.Entity<Person>(e => e.ToTable("People"));

      modelBuilder.Ignore<OrderedEntity<long>>();
      modelBuilder.Ignore<TestEntity<long>>();
    }
  }
}
