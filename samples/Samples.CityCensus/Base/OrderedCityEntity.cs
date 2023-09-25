namespace Kritikos.Samples.CityCensus.Base;

using System;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class OrderedCityEntity<TKey, TEntity> : CityEntity<TKey, TEntity>
  where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable
  where TEntity : CityEntity<TKey, TEntity>
{
  public Guid Order { get; set; }

  internal static new void OnModelCreating(EntityTypeBuilder<TEntity> entity)
  {
    // entity.HasKey(e => e.Id);
    CityEntity<TEntity>.OnModelCreating(entity);
  }
}
