namespace Kritikos.Samples.CityCensus.Model;

using Kritikos.Configuration.Persistence.Extensions;
using Kritikos.Samples.CityCensus.Base;
using Kritikos.Samples.CityCensus.Joins;

using Microsoft.EntityFrameworkCore;

public class Corporation : OrderedCityEntity<long, Corporation>
{
  public string Name { get; set; } = string.Empty;

  public ICollection<County> Counties { get; }
    = new List<County>(0);

  public ICollection<CountyCorporation> CountyCorporations { get; }
    = [];

  internal static void OnModelCreating(ModelBuilder builder)
    => builder.Entity<Corporation>(entity =>
    {
      OnModelCreating(entity);
      entity.ManyToManyWithSkipNavigation<CountyCorporation, Corporation, County>(e => e.Counties, e => e.Corporations);
    });
}
