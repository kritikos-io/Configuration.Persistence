#nullable disable
namespace Kritikos.Configuration.TestData.Base
{
  using System;
  
  using Kritikos.Configuration.Persistence.Contracts.Behavioral;

  public abstract class TestEntity<TKey> : IEntity<TKey>
    where TKey : IComparable, IComparable<TKey>, IEquatable<TKey>
  {
    public TKey Id { get; set; }
  }
}
