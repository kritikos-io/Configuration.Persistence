namespace Kritikos.Samples.CityCensus.Contracts
{
  public interface IOrdered<TKey>
    where TKey : IComparable, IComparable<TKey>
  {
    public TKey Order { get; set; }
  }
}
