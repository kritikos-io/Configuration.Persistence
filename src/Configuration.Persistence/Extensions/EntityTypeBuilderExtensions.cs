namespace Kritikos.Configuration.Persistence.Extensions;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Kritikos.Configuration.Persistence.Contracts.Behavioral;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

public static class EntityTypeBuilderExtensions
{
  /// <summary>
  /// Configures a many-to-many relationship between two entities with primary keys.
  /// Main use case is for join entities, in order to avoid having to create a constructed key when
  /// a composite one can be defined by <typeparamref name="TLeft"/> and <typeparamref name="TRight"/> keys.
  /// </summary>
  /// <remarks>
  /// This method should only be used for simple join entities, when <typeparamref name="TLeft"/> and <typeparamref name="TRight"/>
  /// have primary keys. If any of the two has a composite key, please configure manually.
  /// </remarks>
  /// <param name="entity">The <see cref="EntityTypeBuilder{TEntity}"/> used to configure this entity.</param>
  /// <param name="left">A lambda expression representing the reference navigation property on this entity type that represents
  ///     the relationship (<c>PostTag => PostTag.Post</c>). If no property is specified, the relationship will be
  ///     configured without a navigation property on this end.</param>
  /// <param name="right">A lambda expression representing the second reference navigation property on this entity type that represents
  ///     the relationship (<c>PostTag => PostTag.Tag</c>). If no property is specified, the relationship will be
  ///     configured without a navigation property on this end.</param>
  /// <param name="reverseLeft"> A lambda expression representing the collection navigation property on the other end of this
  ///     relationship (<c>Post => Post.PostTags</c>). If no property is specified, the relationship will be
  ///     configured without a navigation property on the other end of the relationship.</param>
  /// <param name="reverseRight"> A lambda expression representing the second collection navigation property on the other end of this
  ///     relationship (<c>Tag => Tag.PostTags</c>). If no property is specified, the relationship will be
  ///     configured without a navigation property on the other end of the relationship.</param>
  /// <param name="leftKey">The name of the foreign key property for <paramref name="left"/>.</param>
  /// <param name="rightKey">The name of the foreign key property for <paramref name="right"/>.</param>
  /// <typeparam name="TEntity">The <see cref="IJoinEntity{TLeft,TRight}"/> to configure.</typeparam>
  /// <typeparam name="TLeft">The first <see cref="IEntity{TKey}"/> that is part of the many-to-many relationship key.</typeparam>
  /// <typeparam name="TRight">The second <see cref="IEntity{TKey}"/> that is part of the many-to-many relationship key.</typeparam>
  public static void ManyToManyWithJoinEntity<TEntity, TLeft, TRight>(
    this EntityTypeBuilder<TEntity> entity,
    Expression<Func<TEntity, TLeft?>> left,
    Expression<Func<TEntity, TRight?>> right,
    Expression<Func<TLeft, IEnumerable<TEntity>?>>? reverseLeft = null,
    Expression<Func<TRight, IEnumerable<TEntity>?>>? reverseRight = null,
    string? leftKey = null,
    string? rightKey = null)
    where TEntity : class, IJoinEntity<TLeft, TRight>
    where TLeft : class, IEntity
    where TRight : class, IEntity
  {
    ArgumentNullException.ThrowIfNull(entity);
    var leftKeyBuilder = entity.HasOne(left);
    leftKey = string.IsNullOrWhiteSpace(leftKey)
      ? $"{typeof(TLeft).Name}Id"
      : leftKey;
    var leftReferenceCollection = reverseLeft == null
      ? leftKeyBuilder.WithMany()
      : leftKeyBuilder.WithMany(reverseLeft);
    leftReferenceCollection.HasForeignKey(leftKey);

    var rightKeyBuilder = entity.HasOne(right);
    rightKey = string.IsNullOrWhiteSpace(rightKey)
      ? $"{typeof(TRight).Name}Id"
      : rightKey;
    var rightReferenceCollection = reverseLeft == null
      ? rightKeyBuilder.WithMany()
      : rightKeyBuilder.WithMany(reverseRight);
    rightReferenceCollection.HasForeignKey(rightKey);

    entity.HasKey(leftKey, rightKey);
  }

  public static void ManyToManyWithSkipNavigation<TJoin, TLeft, TRight>(
    this EntityTypeBuilder<TLeft> entity,
    Expression<Func<TLeft, IEnumerable<TRight>?>> expression,
    Expression<Func<TRight, IEnumerable<TLeft>?>>? reverse = null)
    where TJoin : class, IJoinEntity<TLeft, TRight>
    where TLeft : class, IEntity
    where TRight : class, IEntity
  {
    ArgumentNullException.ThrowIfNull(entity);
    var builder = entity.HasMany(expression);
    var keyBuilder = reverse == null
      ? builder.WithMany()
      : builder.WithMany(reverse);
    keyBuilder.UsingEntity<TJoin>();
  }
}
