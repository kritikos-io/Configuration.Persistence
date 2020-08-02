namespace Kritikos.Configuration.PersistenceTests
{
	using Xunit;

	public class ModelBuilderExtensionTests
	{
		[Fact]
		public void CheckIOrderedConfiguration()
		{
			var ctx = new TestContext();

			var person = new Person { Name = "Alex" };
			ctx.Add(person);
			ctx.SaveChanges();

			Assert.True(person.Order > 0);

			person = new Person { Name = "Someone" };
			ctx.Add(person);
			ctx.SaveChanges();

			Assert.True(person.Order > 0);
		}
	}
}
