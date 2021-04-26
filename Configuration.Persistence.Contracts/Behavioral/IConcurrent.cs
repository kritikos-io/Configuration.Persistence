namespace Kritikos.Configuration.Persistence.Contracts.Behavioral
{
  /// <summary>
  /// Exposes tracking field to enforce database concurrency.
  /// </summary>
  /// <typeparam name="TKey">Type of RowVersion field in database.</typeparam>
  /// <remarks>Needs overriding of default field name and type in OnModelCreating.</remarks>
  public interface IConcurrent<TKey>
  {
    TKey RowVersion { get; set; }
  }
}
