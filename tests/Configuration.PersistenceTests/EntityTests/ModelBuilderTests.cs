namespace Kritikos.Configuration.PersistenceTests.EntityTests
{
  using System;

  using Kritikos.Configuration.PersistenceTests.Faker;
  using Kritikos.Configuration.TestData;

  using Microsoft.EntityFrameworkCore;

  using Xunit;

  public class ModelBuilderTests
  {
    [SkippableFact]
    public void EntitiesOfType_Interface()
    {
      Skip.If(true, "WIP");
      var builder = new DbContextOptionsBuilder<MigratedDbContext>()
        .UseSqlite("DataSource=transient;mode=memory");
      var ctx = new MigratedDbContext(builder.Options);
      ctx.Database.Migrate();

      var faker = new PersonFaker();
      var people = faker.Generate(10);
      ctx.People.AddRange(people);
      ctx.SaveChanges();

      Assert.All(people, p => Assert.True(p.Order != Guid.Empty));
    }
  }
}
