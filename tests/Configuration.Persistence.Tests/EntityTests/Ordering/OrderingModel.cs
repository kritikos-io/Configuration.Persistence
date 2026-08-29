#pragma warning disable SA1402 // File may only contain a single type
namespace Kritikos.Configuration.Persistence.Tests.EntityTests.Ordering;

using System;

using Kritikos.Configuration.Persistence.Contracts.Behavioral;
using Kritikos.Configuration.Persistence.Extensions;

using Microsoft.EntityFrameworkCore;

public interface ILabelled
{
  string Label { get; set; }
}

public abstract class NumberedEntity : IEntity<long>
{
  public long Id { get; set; }

  public string Number { get; set; } = string.Empty;
}

public class LabelledEntity : IEntity<long>, ILabelled
{
  public long Id { get; set; }

  public string Label { get; set; } = string.Empty;
}

public class LabelledAndNumberedEntity : NumberedEntity, ILabelled
{
  public string Label { get; set; } = string.Empty;
}

public class OnlyNumberedEntity : NumberedEntity;

public class OrderingDbContext : DbContext
{
  public const string LabelDefault = "labelled";
  public const string NumberDefault = "numbered";

  public DbSet<LabelledEntity> Labelled => Set<LabelledEntity>();

  public DbSet<LabelledAndNumberedEntity> LabelledAndNumbered => Set<LabelledAndNumberedEntity>();

  public DbSet<OnlyNumberedEntity> OnlyNumbered => Set<OnlyNumberedEntity>();

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    ArgumentNullException.ThrowIfNull(optionsBuilder);

    if (!optionsBuilder.IsConfigured)
    {
      optionsBuilder.UseSqlite("DataSource=ordering_model;mode=memory");
    }

    base.OnConfiguring(optionsBuilder);
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    ArgumentNullException.ThrowIfNull(modelBuilder);
    base.OnModelCreating(modelBuilder);

    modelBuilder.EntitiesImplementing<ILabelled>(
      entity => entity.Property<string>(nameof(ILabelled.Label)).HasDefaultValue(LabelDefault));

    modelBuilder.EntitiesOfType<NumberedEntity>(
      entity => entity.Property(x => x.Number).HasDefaultValue(NumberDefault));
  }
}
