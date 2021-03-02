namespace Kritikos.Configuration.PersistenceTests
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using Kritikos.Configuration.Persistence.Extensions;
	using Kritikos.Configuration.Persistence.Interceptors;
	using Kritikos.Configuration.TestData;

	using Microsoft.Data.Sqlite;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Diagnostics;

	public class DbContextFixture : IDisposable
	{
		private readonly ConcurrentDictionary<string, SqliteConnection> sqlConnections = new();

		public static readonly TimestampSaveChangesInterceptor TimestampInterceptor = new();

		public static readonly ReadOnlyDbCommandInterceptor ReadOnlyInterceptor = new();

		public async Task<MigratedDbContext> GetContext(string databaseName, params IInterceptor[] interceptors)
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

			var opts = new DbContextOptionsBuilder<MigratedDbContext>()
				.UseSqlite(sqlConnections[databaseName])
				.EnableCommonOptions(true)
				.AddInterceptors(interceptors)
				.Options;

			return new MigratedDbContext(opts);
		}

		public static IEnumerable<int> Naturals()
		{
			var i = 0;
			while (true)
			{
				yield return i++;
			}
		}

		public static List<T> GetEntries<T>(int count = 10)
			where T : class, new()
			=> Naturals().Take(count).Select(x => new T()).ToList();

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
