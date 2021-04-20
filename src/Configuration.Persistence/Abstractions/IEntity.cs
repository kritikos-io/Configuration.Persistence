namespace Kritikos.Configuration.Persistence.Abstractions
{
  using System;

  using Kritikos.Configuration.Persistence.Dto.Abstractions;

  /// <summary>
  /// Exposes basic behavior for relational entity persistence.
  /// </summary>
  /// <typeparam name="TKey">Type of primary key.</typeparam>
  [Obsolete("Replaced by IIdentity<TKey>")]
  public interface IEntity<TKey> : IIdentity<TKey>
    where TKey : IComparable, IComparable<TKey>, IEquatable<TKey>
  {
  }
}
