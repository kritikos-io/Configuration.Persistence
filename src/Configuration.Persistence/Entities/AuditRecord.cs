namespace Kritikos.Configuration.Persistence.Entities;

using System;

using Kritikos.Configuration.Persistence.Contracts.Behavioral;

using Microsoft.EntityFrameworkCore;

/// <summary>
/// A single audit trail entry describing one tracked change to one entity.
/// </summary>
public class AuditRecord : IEntity<long>, ICreateTimestamped, ICreateAuditable<Guid>
{
  /// <inheritdoc />
  public long Id { get; set; }

  /// <inheritdoc />
  public DateTime CreatedAt { get; set; }

  /// <inheritdoc />
  public Guid CreatedBy { get; set; }

  /// <summary>
  /// Gets the name of the table the audited entity is mapped to.
  /// </summary>
  public string Table { get; init; } = string.Empty;

  /// <summary>
  /// Gets the serialized primary key of the audited entity.
  /// </summary>
  public string Key { get; init; } = string.Empty;

  /// <summary>
  /// Gets the kind of change that produced this entry.
  /// </summary>
  public EntityState Modification { get; init; }

  /// <summary>
  /// Gets the serialized property values before the change.
  /// </summary>
  public string OldValues { get; init; } = string.Empty;

  /// <summary>
  /// Gets the serialized property values after the change.
  /// </summary>
  public string NewValues { get; init; } = string.Empty;

  /// <summary>
  /// Configures the audit record entity on the supplied model.
  /// </summary>
  /// <param name="builder">The builder being used to construct the model for the context.</param>
  /// <exception cref="ArgumentNullException"><paramref name="builder"/> is null.</exception>
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
