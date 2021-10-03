namespace Kritikos.Samples.CityCensus.Provider
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  using Bogus;

  using Kritikos.Samples.CityCensus.Model;

  public class CountyProvider : Faker<County>
  {
    private CountyProvider()
    {
      RuleFor(o => o.Name, f => f.Address.County());
    }

    public static CountyProvider Provider { get; } = new CountyProvider();
  }
}
