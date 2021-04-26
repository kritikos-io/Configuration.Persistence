namespace Kritikos.Configuration.Persistence.Extensions
{
  using Kritikos.Configuration.Persistence.Contracts.Behavioral;

  using Microsoft.EntityFrameworkCore;

  public static class DbContextExtensions
  {
    public static void UpdateConcurrencyToken<TKey>(this DbContext ctx, IConcurrent<TKey> entity, TKey rowVersion)
      => ctx.Entry(entity).Property(x => x.RowVersion).OriginalValue = rowVersion;
  }
}
