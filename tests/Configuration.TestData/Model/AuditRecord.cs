namespace Kritikos.Configuration.TestData.Model
{
  using System;

  using Kritikos.Configuration.Persistence.Contracts.Behavioral;

  public class AuditRecord : IEntity<long>, ITimestamped, IAuditable<Guid>
  {
    public long Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Guid CreatedBy { get; set; }

    public Guid UpdatedBy { get; set; }
  }
}
