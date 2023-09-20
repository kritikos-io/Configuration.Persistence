#nullable disable
namespace Kritikos.Samples.CityCensus;

using System;

using Kritikos.Configuration.Persistence.Contracts;
using Kritikos.Configuration.Persistence.Entities;
using Kritikos.Configuration.Persistence.Extensions;
using Kritikos.Samples.CityCensus.Base;
using Kritikos.Samples.CityCensus.Contracts;
using Kritikos.Samples.CityCensus.Joins;
using Kritikos.Samples.CityCensus.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;

public class CityCensusTrailDbContext : DbContext, IAuditTrailDbContext<AuditRecord>
{
  public CityCensusTrailDbContext()
  {
  }

  public CityCensusTrailDbContext(DbContextOptions<CityCensusTrailDbContext> options)
    : base(options)
  {
  }

  public DbSet<AuditRecord> AuditRecords { get; set; } = null!;

  public DbSet<County> Counties { get; set; }

  public DbSet<Corporation> Corporations { get; set; }

  public DbSet<CountyCorporation> CountyCorporations { get; set; }

  public DbSet<Person> People { get; set; }

  /// <inheritdoc />
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    ArgumentNullException.ThrowIfNull(optionsBuilder);

    if (!optionsBuilder.IsConfigured)
    {
      optionsBuilder.UseSqlite("DataSource=transient;mode=memory;cache=shared")
        .EnableCommonOptions(true);
    }

    base.OnConfiguring(optionsBuilder);
  }

  /// <inheritdoc />
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    ArgumentNullException.ThrowIfNull(modelBuilder);
    base.OnModelCreating(modelBuilder);

    modelBuilder.ApplyEntityConfiguration();

    modelBuilder.EntitiesImplementing<IOrdered<Guid>>(entity =>
    {
      entity.Property(typeof(Guid), nameof(IOrdered<Guid>.Order))
        .HasValueGenerator((_, _) => new GuidValueGenerator());
    });

    modelBuilder.EntitiesOfType<OrderedCityEntity<long, Corporation>>(entity =>
    {
      entity.Property(e => e.Order)
        .HasValueGenerator((_, _) => new GuidValueGenerator());
    });
  }
}
