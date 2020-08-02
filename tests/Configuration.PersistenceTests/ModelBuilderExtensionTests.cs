using System;
using System.Collections.Generic;
using System.Text;

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

			Assert.Equal(1, person.Order);

			person = new Person { Name = "Someone" };
			ctx.Add(person);
			ctx.SaveChanges();

			Assert.Equal(2, person.Order);
		}
	}
}
