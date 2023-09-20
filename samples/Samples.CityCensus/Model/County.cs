namespace Kritikos.Samples.CityCensus.Model;

using System;

using Kritikos.Configuration.Persistence.Contracts;
using Kritikos.Configuration.Persistence.Contracts.Behavioral;
using Kritikos.Samples.CityCensus.Base;
using Kritikos.Samples.CityCensus.Contracts;

using Microsoft.EntityFrameworkCore;

public class County : CityEntity<long, County>, ITimestamped, IOrdered<Guid>, IConfigurableEntity
{
  public string Name { get; set; } = string.Empty;

  public DateTime CreatedAt { get; set; }

  public DateTime UpdatedAt { get; set; }

  public Guid Order { get; set; }

  public ICollection<Corporation> Corporations { get; }
    = new List<Corporation>(0);

  internal static void OnModelCreating(ModelBuilder builder)
  {
    ArgumentNullException.ThrowIfNull(builder);

    builder.Entity<County>(entity =>
    {
      OnModelCreating(entity);
    });
  }
}
