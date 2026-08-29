namespace Kritikos.Configuration.Persistence.Tests.EntityTests;

using System;
using System.Linq;
using System.Threading.Tasks;

using Kritikos.Configuration.Persistence.Entities;

using Microsoft.EntityFrameworkCore;

public class AuditRecordTests
{
  [Test]
  public async Task OnModelCreating_ValidModelBuilder_ConfiguresIdAsPrimaryKey()
  {
    var builder = new ModelBuilder();

    AuditRecord.OnModelCreating(builder);

    var key = builder.Model.FindEntityType(typeof(AuditRecord))?.FindPrimaryKey();

    await Assert.That(key).IsNotNull();
    await Assert.That(key!.Properties.Select(x => x.Name).ToList()).IsEquivalentTo([nameof(AuditRecord.Id)]);
  }

  [Test]
  public async Task OnModelCreating_NullModelBuilder_ThrowsArgumentNullException()
    => await Assert.That(() => AuditRecord.OnModelCreating(null!))
      .Throws<ArgumentNullException>();
}
