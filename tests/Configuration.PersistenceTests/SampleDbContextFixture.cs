namespace Kritikos.Configuration.PersistenceTests
{
  using System;
  using System.Collections.Concurrent;
  using System.Threading.Tasks;

  using Kritikos.Configuration.Persistence.Extensions;
  using Kritikos.Samples.CityCensus;

  using Microsoft.Data.Sqlite;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore.Diagnostics;

  using Xunit.Abstractions;

  public class SampleDbContextFixture : IDisposable
  {
    private readonly ConcurrentDictionary<string, SqliteConnection> sqlConnections = new();

    public async Task<CityCensusTrailDbContext> GetContext(string databaseName, params IInterceptor[] interceptors)
    {
      var sqlConnection = new SqliteConnection($"DataSource={databaseName};mode=memory;cache=shared");
      if (!sqlConnections.TryAdd(databaseName, sqlConnection))
      {
        await sqlConnection.DisposeAsync();
      }
      else
      {
        await sqlConnection.OpenAsync();
      }

      var opts = new DbContextOptionsBuilder<CityCensusTrailDbContext>()
        .UseSqlite(sqlConnections[databaseName])
        .EnableCommonOptions(true)
        .AddInterceptors(interceptors)
        .Options;

      return new CityCensusTrailDbContext(opts);
    }

    public void Dispose()
    {
      foreach (var (key, sqlite) in sqlConnections)
      {
        sqlite.Dispose();
        sqlConnections.TryRemove(key, out _);
      }

      GC.SuppressFinalize(this);
    }
  }
}
