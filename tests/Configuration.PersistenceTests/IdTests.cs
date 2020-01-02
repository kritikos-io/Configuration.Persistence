namespace Kritikos.Configuration.PersistenceTests
{
	using System.Diagnostics.CodeAnalysis;
	using System.Linq;

	using Xunit;

	[ExcludeFromCodeCoverage]
	public class IdTests
	{
		[Fact]
		public void IdInsertionTests()
		{
			var ctx = new TestContext();

			var person = new Person { Name = "Alex" };
			ctx.Add(person);
			ctx.SaveChanges();

			Assert.True(person.Id > 0);
		}
	}
}
