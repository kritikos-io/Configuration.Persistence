namespace Kritikos.Configuration.Persistence.Contracts.Behavioral;

/// <summary>
/// Interface handling soft deletes to enable recovery of deleted entries.
/// </summary>
public interface ISoftDeletable
{
  /// <summary>
  /// Gets or sets a value indicating whether this entity has been soft deleted.
  /// </summary>
  public bool IsDeleted { get; set; }

  /// <summary>
  /// Gets or sets the time this entity was soft deleted, or <see langword="null"/> if it is still active.
  /// </summary>
  public DateTime? DeletedAt { get; set; }
}
