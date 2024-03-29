// ReSharper disable NonReadonlyMemberInGetHashCode
namespace Kritikos.Configuration.Persistence.Contracts.Base;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Kritikos.Configuration.Persistence.Contracts.Behavioral;

/// <summary>
/// Base model for Dtos to allow comparsions after updating from the server.
/// </summary>
/// <typeparam name="TKey">Type of primary identity.</typeparam>
[ExcludeFromCodeCoverage]
public abstract class Model<TKey> : IEntity<TKey>, IEquatable<Model<TKey>>
  where TKey : IComparable<TKey>, IEquatable<TKey>
{
  public TKey Id { get; set; } = default!;

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
