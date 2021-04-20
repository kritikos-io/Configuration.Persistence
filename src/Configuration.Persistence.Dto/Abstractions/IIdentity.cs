namespace Kritikos.Configuration.Persistence.Dto.Abstractions
{
  using System;

  /// <summary>
  /// Exposes basic behavior for identifying a given entity.
  /// </summary>
  /// <typeparam name="TKey">Type of primary key.</typeparam>
  public interface IIdentity<TKey>
    where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable
  {
    TKey Id { get; init; }
  }
}
