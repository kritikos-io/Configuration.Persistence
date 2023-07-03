namespace Kritikos.Samples.CityCensus;

using Bogus;

using Kritikos.Samples.CityCensus.Model;

using Person = Kritikos.Samples.CityCensus.Model.Person;

public static class CityDataFaker
{
  public static Faker<Person> People { get; }
    = new Faker<Person>()
      .RuleFor(o => o.Email, (f, o) => f.Internet.Email(o.FirstName, o.LastName))
      .RuleFor(o => o.FirstName, f => f.Person.FirstName)
      .RuleFor(o => o.LastName, f => f.Person.LastName);

  public static Faker<County> Counties { get; }
    = new Faker<County>()
      .RuleFor(o => o.Name, f => f.Address.County());
}
