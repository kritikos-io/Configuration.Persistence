namespace Kritikos.Configuration.Persistence.Contracts.Behavioral;

/// <summary>
/// Interface handling soft deletes to enable recovery of deleted entries.
/// </summary>
public interface ISoftDeletable
{
  public bool IsDeleted { get; set; }
}
