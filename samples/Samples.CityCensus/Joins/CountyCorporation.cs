namespace Kritikos.Samples.CityCensus.Joins
{
  using Kritikos.Configuration.Persistence.Extensions;
  using Kritikos.Samples.CityCensus.Base;
  using Kritikos.Samples.CityCensus.Model;

  using Microsoft.EntityFrameworkCore;

  public class CountyCorporation : CityEntity<long, CountyCorporation>
  {
#nullable disable
    public County County { get; set; }

    public Corporation Corporation { get; set; }

    internal static void OnModelCreating(ModelBuilder builder)
      => builder.Entity<CountyCorporation>(entity =>
      {
        OnModelCreating(entity);

        entity.ManyToManyTable(x => x.County, x => x.Corporation);
      });
  }
}
