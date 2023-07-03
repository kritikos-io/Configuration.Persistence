namespace Kritikos.Configuration.Persistence.Extensions;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Kritikos.Configuration.Persistence.Contracts.Behavioral;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

public static class EntityTypeBuilderExtensions
{
  // TODO: Finish documentation
  public static void ManyToManyTable<TEntity, TLeft, TRight>(
    this EntityTypeBuilder<TEntity> entity,
    Expression<Func<TEntity, TLeft?>>? left,
    Expression<Func<TEntity, TRight?>> right,
    Expression<Func<TLeft, IEnumerable<TEntity>?>>? reverseLeft = null,
    Expression<Func<TRight, IEnumerable<TEntity>?>>? reverseRight = null,
    string? leftKey = null,
    string? rightKey = null)
    where TEntity : class, IEntity
    where TLeft : class, IEntity
    where TRight : class, IEntity
  {
    ArgumentNullException.ThrowIfNull(entity);
    var leftKeyBuilder = entity.HasOne(left);
    leftKey = string.IsNullOrWhiteSpace(leftKey)
      ? $"{typeof(TLeft).Name}Id"
      : leftKey;
    if (reverseLeft != null)
    {
      leftKeyBuilder.WithMany(reverseLeft).HasForeignKey(leftKey);
    }
    else
    {
      leftKeyBuilder.WithMany().HasForeignKey(leftKey);
    }

    var rightKeyBuilder = entity.HasOne(right);
    rightKey = string.IsNullOrWhiteSpace(rightKey)
      ? $"{typeof(TRight).Name}Id"
      : rightKey;
    if (reverseRight != null)
    {
      rightKeyBuilder.WithMany(reverseRight).HasForeignKey(rightKey);
    }
    else
    {
      rightKeyBuilder.WithMany().HasForeignKey(rightKey);
    }

    entity.HasKey(leftKey, rightKey);
  }
}
