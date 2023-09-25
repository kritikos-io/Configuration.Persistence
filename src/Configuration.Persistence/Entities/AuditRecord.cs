namespace Kritikos.Configuration.Persistence.Entities;

using System;

using Kritikos.Configuration.Persistence.Contracts.Behavioral;

using Microsoft.EntityFrameworkCore;

public class AuditRecord : IEntity<long>, ICreateTimestamped, ICreateAuditable<Guid>
{
  public long Id { get; set; }

  public DateTime CreatedAt { get; set; }

  public Guid CreatedBy { get; set; }

  public string Table { get; init; } = string.Empty;

  public string Key { get; init; } = string.Empty;

  public EntityState Modification { get; init; }

  public string OldValues { get; init; } = string.Empty;

  public string NewValues { get; init; } = string.Empty;

  public static void OnModelCreating(ModelBuilder builder)
  {
    ArgumentNullException.ThrowIfNull(builder);
    builder.Entity<AuditRecord>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Modification)
        .HasConversion<string>();
    });
  }
}
