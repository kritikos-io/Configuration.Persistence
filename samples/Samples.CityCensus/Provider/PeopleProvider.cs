namespace Kritikos.Samples.CityCensus.Provider
{
  using Bogus;

  using Person = Kritikos.Samples.CityCensus.Model.Person;

  public class PeopleProvider : Faker<Person>
  {
    private PeopleProvider()
    {
      RuleFor(o => o.Email, (f, o) => f.Internet.Email(o.FirstName, o.LastName));
      RuleFor(o => o.FirstName, f => f.Person.FirstName);
      RuleFor(o => o.LastName, f => f.Person.LastName);
    }

    public static PeopleProvider Provider { get; } = new PeopleProvider();
  }
}
