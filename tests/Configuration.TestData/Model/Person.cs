namespace Kritikos.Configuration.TestData.Model
{
  using System;

  using Kritikos.Configuration.TestData.Base;

  public class Person : TestEntity<long>, IObfuscated
  {
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    #region Implementation of IObfuscated

    /// <inheritdoc />
    public Guid Order { get; set; }

    #endregion
  }
}
