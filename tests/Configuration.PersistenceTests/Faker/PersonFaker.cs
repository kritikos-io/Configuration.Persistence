namespace Kritikos.Configuration.PersistenceTests.Faker
{
  using Bogus;

  public sealed class PersonFaker : Faker<Kritikos.Configuration.TestData.Model.Person>
  {
    public PersonFaker(int orderCounterStart = 0)
    {
      OrderCount = orderCounterStart;
      RuleFor(o => o.Email, (f, o) => f.Internet.Email(o.FirstName, o.LastName));
      RuleFor(o => o.FirstName, f => f.Person.FirstName);
      RuleFor(o => o.LastName, f => f.Person.LastName);

      // RuleFor(o => o.Order, f => OrderCount++);
    }

    public int OrderCount { get; private set; }
  }
}
