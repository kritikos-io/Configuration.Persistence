namespace Kritikos.Configuration.PersistenceTests
{
	using System;
	using System.Diagnostics.CodeAnalysis;

	using Kritikos.Configuration.Persistence.Base;

	using Microsoft.EntityFrameworkCore;

	[ExcludeFromCodeCoverage]
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
	public class TestContext : DbContext
	{
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
	}

	public class Person : ConcurrentEntity<long>
	{
		public string Name { get; set; } = string.Empty;
	}
}
