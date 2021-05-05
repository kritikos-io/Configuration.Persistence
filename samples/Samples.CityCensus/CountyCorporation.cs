namespace Kritikos.Samples.CityCensus
{
  using Kritikos.Samples.CityCensus.Base;

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
        entity.HasOne(e => e.County)
          .WithMany()
          .HasForeignKey("CountyId");
        entity.HasOne(e => e.Corporation)
          .WithMany()
          .HasForeignKey("CorporationId");
        entity.HasKey("CountyId", "CorporationId");
      });
  }
}
