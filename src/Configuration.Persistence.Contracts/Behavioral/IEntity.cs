#pragma warning disable SA1402 // File may only contain a single type
namespace Kritikos.Configuration.Persistence.Contracts.Behavioral
{
  using System;

  /// <summary>
  /// Marker interface used to decorate join entities.
  /// </summary>
  public interface IEntity
  {
  }

  /// <summary>
  /// Exposes basic behavior for relational entity persistence.
  /// </summary>
  /// <typeparam name="TKey">Type of primary key.</typeparam>
  public interface IEntity<TKey> : IEntity
    where TKey : IComparable, IComparable<TKey>, IEquatable<TKey>
  {
    TKey Id { get; set; }
  }
}
