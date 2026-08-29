// ReSharper disable NonReadonlyMemberInGetHashCode
namespace Kritikos.Configuration.Persistence.Contracts.Base;

using System;
using System.Collections.Generic;

using Kritikos.Configuration.Persistence.Contracts.Behavioral;

/// <summary>
/// Base model for Dtos to allow comparisons after updating from the server.
/// </summary>
/// <typeparam name="TKey">Type of primary identity.</typeparam>
public abstract class Model<TKey> : IEntity<TKey>, IEquatable<Model<TKey>>
  where TKey : IComparable<TKey>, IEquatable<TKey>
{
  /// <inheritdoc />
  public TKey Id { get; set; } = default!;

  /// <summary>
  /// Determines whether two models share an identity.
  /// </summary>
  /// <param name="left">The model on the left of the operator.</param>
  /// <param name="right">The model on the right of the operator.</param>
  /// <returns><see langword="true"/> if both are null or both carry the same <see cref="Id"/>, otherwise <see langword="false"/>.</returns>
  public static bool operator ==(Model<TKey>? left, Model<TKey>? right)
    => left is null
      ? right is null
      : left.Equals(right);

  /// <summary>
  /// Determines whether two models differ in identity.
  /// </summary>
  /// <param name="left">The model on the left of the operator.</param>
  /// <param name="right">The model on the right of the operator.</param>
  /// <returns><see langword="true"/> if exactly one is null or they carry different <see cref="Id"/> values, otherwise <see langword="false"/>.</returns>
  public static bool operator !=(Model<TKey>? left, Model<TKey>? right)
    => !(left == right);

  /// <inheritdoc />
  public virtual bool Equals(Model<TKey>? other)
    => other is not null
       && (ReferenceEquals(this, other) || EqualityComparer<TKey>.Default.Equals(this.Id, other.Id));

  /// <inheritdoc />
  public override bool Equals(object? obj)
    => obj is not null
       && (ReferenceEquals(this, obj) || (obj is Model<TKey> m && Equals(m)));

  /// <inheritdoc />
  public override int GetHashCode()
    => EqualityComparer<TKey>.Default.GetHashCode(Id);
}
