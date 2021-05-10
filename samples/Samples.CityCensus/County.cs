namespace Kritikos.Samples.CityCensus
{
  using Kritikos.Samples.CityCensus.Base;

  using Microsoft.EntityFrameworkCore;

  public class County : CityEntity<long, County>
  {
    public string Name { get; set; } = string.Empty;

    internal static void OnModelCreating(ModelBuilder builder)
      => builder.Entity<County>(entity =>
      {
        OnModelCreating(entity);
      });
  }
}
