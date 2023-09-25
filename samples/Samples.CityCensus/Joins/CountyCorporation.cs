#nullable disable
namespace Kritikos.Samples.CityCensus.Joins;

using Kritikos.Configuration.Persistence.Contracts.Behavioral;
using Kritikos.Configuration.Persistence.Extensions;
using Kritikos.Samples.CityCensus.Base;
using Kritikos.Samples.CityCensus.Model;

using Microsoft.EntityFrameworkCore;

public class CountyCorporation : CityEntity<long, CountyCorporation>, IJoinEntity<Corporation, long, County, long>
{
  public County County { get; set; }

  public Corporation Corporation { get; set; }

  internal static void OnModelCreating(ModelBuilder builder)
    => builder.Entity<CountyCorporation>(entity =>
    {
      OnModelCreating(entity);

      entity.ManyToManyWithJoinEntity(x => x.Corporation, x => x.County);
    });
}
