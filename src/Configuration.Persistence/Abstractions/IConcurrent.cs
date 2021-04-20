namespace Kritikos.Configuration.Persistence.Abstractions
{
  /// <summary>
  /// Exposes tracking field to enforce database concurrency for Microsoft SQL Server.
  /// </summary>
  public interface IConcurrent
  {
    byte[] RowVersion { get; set; }
  }
}
