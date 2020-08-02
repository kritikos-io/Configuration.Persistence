namespace Kritikos.Configuration.PersistenceTests
{
	using System;

	using Xunit;

	public class ChangeTrackerExtensionTests
	{
		[Fact]
		public void EnsureAudited()
		{
			var ctx = new TestContext();

			var person = new Person { Name = "Alex" };

			ctx.Add(person);
			ctx.SaveChanges();

			Assert.True(person.CreatedBy == TestContext.DefaultUser, "IAuditable.CreatedBy was not set properly!");
			Assert.True(person.UpdatedBy == TestContext.DefaultUser, "IAuditable.UpdatedBy was not set properly!");
		}

		[Fact]
		public void EnsureTimestamped()
		{
			var ctx = new TestContext();

			var person = new Person { Name = "Alex" };
			var now = DateTimeOffset.Now;

			ctx.Add(person);
			var records = ctx.SaveChanges(now);


			Assert.Equal(1,records);
			Assert.True(person.CreatedAt == now, "ITimestamped.CreatedBy was not updated!");
			Assert.True(person.UpdatedAt == now, "ITimestamped.UpdatedBy was not updated!");
		}
	}
}
