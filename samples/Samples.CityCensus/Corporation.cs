namespace Kritikos.Samples.CityCensus
{
  using Kritikos.Samples.CityCensus.Base;

  using Microsoft.EntityFrameworkCore;

  public class Corporation : CityEntity<long, Corporation>
  {
    public string Name { get; set; } = string.Empty;

    internal static void OnModelCreating(ModelBuilder builder)
      => builder.Entity<Corporation>(entity =>
      {
        OnModelCreating(entity);
      });
  }
}
