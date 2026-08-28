namespace Kritikos.Configuration.PersistenceTests.EntityTests.Concurrency;

using Kritikos.Configuration.Persistence.Contracts;
using Kritikos.Configuration.Persistence.Contracts.Behavioral;

public class PostgreSqlConcurrentEntity : IEntity<long>, IPostgreSqlConcurrent
{
  public long Id { get; set; }

  public uint RowVersion { get; set; }
}
