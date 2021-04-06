namespace Kritikos.Configuration.Persistence.Abstractions
{
  using System;

  /// <summary>
  /// Exposes basic behavior for relational entity persistence.
  /// </summary>
  /// <typeparam name="TKey">Type of primary key.</typeparam>
  public interface IEntity<TKey>
    where TKey : IComparable, IComparable<TKey>, IEquatable<TKey>
  {
    TKey Id { get; set; }
  }
}
