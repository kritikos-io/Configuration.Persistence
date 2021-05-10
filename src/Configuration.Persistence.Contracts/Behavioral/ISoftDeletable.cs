namespace Kritikos.Configuration.Persistence.Contracts.Behavioral
{
  public interface ISoftDeletable
  {
    public bool IsDeleted { get; set; }
  }
}
