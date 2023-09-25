namespace Kritikos.Samples.CityCensus.Contracts;

using System;

public interface IOrdered<TKey>
  where TKey : IComparable, IComparable<TKey>
{
  public TKey Order { get; set; }
}
