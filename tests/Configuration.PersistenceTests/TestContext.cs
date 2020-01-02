namespace Kritikos.Configuration.PersistenceTests
{
	using System.Diagnostics.CodeAnalysis;

	using Kritikos.Configuration.Persistence.Base;

	using Microsoft.EntityFrameworkCore;

	[ExcludeFromCodeCoverage]
    public class TestContext : DbContext
    {
		public DbSet<Person> People { get; set; }

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
		public string Name { get; set; }
	}
}
