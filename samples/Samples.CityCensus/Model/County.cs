namespace Kritikos.Samples.CityCensus.Model;

using System;

using Kritikos.Configuration.Persistence.Contracts.Behavioral;
using Kritikos.Samples.CityCensus.Base;
using Kritikos.Samples.CityCensus.Contracts;

using Microsoft.EntityFrameworkCore;

public class County : CityEntity<long, County>, ITimestamped, IOrdered<Guid>
{
  public string Name { get; set; } = string.Empty;

  public DateTime CreatedAt { get; set; }

  public DateTime UpdatedAt { get; set; }

  public Guid Order { get; set; }

  internal static void OnModelCreating(ModelBuilder builder)
    => builder.Entity<County>(entity =>
    {
      OnModelCreating(entity);
    });
}
