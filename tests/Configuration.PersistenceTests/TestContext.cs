namespace Kritikos.Configuration.PersistenceTests
{
	using System;
	using System.Diagnostics.CodeAnalysis;
	using System.Linq;

	using Kritikos.Configuration.Persistence;
	using Kritikos.Configuration.Persistence.Abstractions;
	using Kritikos.Configuration.Persistence.Base;

	using Microsoft.EntityFrameworkCore;

	[ExcludeFromCodeCoverage]
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
	public class TestContext : DbContext
	{
		public const string DefaultUser = "Test";

#nullable disable
		public DbSet<Person> People { get; private set; }
#nullable enable
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				optionsBuilder.UseInMemoryDatabase("inMemory");
			}
		}


		public int SaveChanges(DateTimeOffset? now)
		{
			now ??= DateTimeOffset.Now;
			
			ChangeTracker.Entries()
				.ToList()
				.TimeStampEntityEntries(now.Value)
				.AuditEntities(DefaultUser);

			return base.SaveChanges();
		}

		#region Overrides of DbContext

		/// <inheritdoc />
		public override int SaveChanges()
		{
			var now = DateTimeOffset.Now;
			ChangeTracker.Entries()
				.ToList()
				.TimeStampEntityEntries(now)
				.AuditEntities(DefaultUser);

			return base.SaveChanges();
		}

		#endregion
	}

	public class Person : ConcurrentEntity<long>, IAuditable<string>, ITimestamped
	{
		public string Name { get; set; } = string.Empty;

		#region Implementation of IAuditable<string>

		/// <inheritdoc />
		public string CreatedBy { get; set; } = string.Empty;

		/// <inheritdoc />
		public string UpdatedBy { get; set; } = string.Empty;

		#endregion

		#region Implementation of ITimestamped

		/// <inheritdoc />
		public DateTimeOffset CreatedAt { get; set; }

		/// <inheritdoc />
		public DateTimeOffset UpdatedAt { get; set; }

		#endregion
	}
}
