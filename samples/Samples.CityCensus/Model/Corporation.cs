namespace Kritikos.Samples.CityCensus.Model;

using Kritikos.Samples.CityCensus.Base;

using Microsoft.EntityFrameworkCore;

public class Corporation : OrderedCityEntity<long, Corporation>
{
  public string Name { get; set; } = string.Empty;

  internal static void OnModelCreating(ModelBuilder builder)
    => builder.Entity<Corporation>(entity =>
    {
      OnModelCreating(entity);
    });
}
