#pragma warning disable SA1402 // File may only contain a single type
#nullable disable
namespace Kritikos.Samples.CityCensus.Base;

using System;

using Kritikos.Configuration.Persistence.Contracts.Behavioral;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

public abstract class CityEntity<TKey, TEntity> : CityEntity<TEntity>, IEntity<TKey>
  where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable
  where TEntity : CityEntity<TKey, TEntity>
{
  public TKey Id { get; set; }

  internal static new void OnModelCreating(EntityTypeBuilder<TEntity> entity)
  {
    entity.HasKey(e => e.Id);
    CityEntity<TEntity>.OnModelCreating(entity);
  }
}

public abstract class CityEntity<TEntity> : ITraceableAudit
  where TEntity : class
{
  internal static void OnModelCreating(EntityTypeBuilder<TEntity> entity)
  {
    ArgumentNullException.ThrowIfNull(entity);
  }
}
