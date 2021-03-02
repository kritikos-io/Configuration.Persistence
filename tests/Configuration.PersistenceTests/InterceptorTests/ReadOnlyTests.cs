namespace Kritikos.Configuration.PersistenceTests.InterceptorTests
{
	using System.Linq;
	using System.Threading.Tasks;

	using FluentAssertions;

	using Kritikos.Configuration.PersistenceTests.Faker;

	using Microsoft.EntityFrameworkCore;

	using Xunit;

	public class ReadOnlyTests : IClassFixture<DbContextFixture>
	{
		private readonly DbContextFixture fixture;

		public ReadOnlyTests(DbContextFixture fixture) => this.fixture = fixture;

		[Fact]
		public async Task Save_entries_are_unchanged()
		{
			var faker = new PersonFaker();
			var people = faker.Generate(10);
			await using var ctx = await fixture.GetContext("readonly");
			await ctx.Database.MigrateAsync();

			ctx.People.AddRange(people);
			await ctx.SaveChangesAsync();

			await using var readOnly =
				await fixture.GetContext("readonly", DbContextFixture.ReadOnlyInterceptor);

			var newPeople = await ctx.People.ToListAsync();
			foreach (var person in newPeople)
			{
				person.FirstName = string.Empty;
				person.LastName = string.Empty;
			}

			_ = newPeople.Select(x => x.FirstName.Should().BeNullOrWhiteSpace());
			_ = newPeople.Select(x => x.LastName.Should().BeNullOrWhiteSpace());

			await readOnly.SaveChangesAsync();
			newPeople = await readOnly.People.ToListAsync();

			_=newPeople.Select(x => x.FirstName.Should().NotBeNullOrWhiteSpace());
			_ = newPeople.Select(x => x.LastName.Should().NotBeNullOrWhiteSpace());
		}
	}
}
