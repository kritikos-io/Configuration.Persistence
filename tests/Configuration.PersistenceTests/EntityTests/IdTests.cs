namespace Kritikos.Configuration.PersistenceTests.EntityTests
{
	using System.Diagnostics.CodeAnalysis;

	using Xunit;

	[ExcludeFromCodeCoverage]
	public class IdTests
	{
		[Fact]
		public void Insertion()
		{
			var ctx = new TestContext();

			var person = new Person { Name = "Alex" };
			ctx.Add(person);
			ctx.SaveChanges();

			Assert.True(person.Id > 0);
		}
	}
}
