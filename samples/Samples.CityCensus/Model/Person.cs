namespace Kritikos.Samples.CityCensus.Model;

using System;

using Kritikos.Configuration.Persistence.Contracts.Behavioral;
using Kritikos.Samples.CityCensus.Base;

public class Person : CityEntity<long, Person>, IAuditable<Guid>, ISoftDeletable
{
  public string FirstName { get; set; } = string.Empty;

  public string LastName { get; set; } = string.Empty;

  public string Email { get; set; } = string.Empty;

  public County? County { get; set; }

  public Guid CreatedBy { get; set; }

  public Guid UpdatedBy { get; set; }

  /// <inheritdoc />
  public bool IsDeleted { get; set; }

  public DateTime? DeletedAt { get; set; }
}
