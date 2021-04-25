namespace Kritikos.Configuration.Persistence.Extensions
{
  using Kritikos.Configuration.Persistence.Abstractions;

  using Microsoft.EntityFrameworkCore;

  public static class DbContextExtensions
  {
    public static void UpdateConcurrencyToken(this DbContext ctx, IConcurrent entity, byte[] rowVersion)
      => ctx.Entry(entity).Property(x => x.RowVersion).OriginalValue = rowVersion;
  }
}
