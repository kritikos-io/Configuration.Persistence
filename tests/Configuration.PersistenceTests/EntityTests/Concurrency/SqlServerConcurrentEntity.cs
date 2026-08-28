namespace Kritikos.Configuration.PersistenceTests.EntityTests.Concurrency;

using Kritikos.Configuration.Persistence.Contracts;
using Kritikos.Configuration.Persistence.Contracts.Behavioral;

public class SqlServerConcurrentEntity : IEntity<long>, ISqlServerConcurrent
{
  public long Id { get; set; }

  public byte[] RowVersion { get; set; } = [];
}
