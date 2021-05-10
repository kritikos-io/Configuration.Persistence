namespace Kritikos.Configuration.PersistenceTests.InterceptorTests
{
  using System;
  using System.Linq;
  using System.Threading.Tasks;

  using FluentAssertions;

  using Kritikos.Configuration.Persistence.Interceptors;
  using Kritikos.Configuration.Persistence.Interceptors.SaveChanges;
  using Kritikos.Configuration.Persistence.Interceptors.Services;
  using Kritikos.Configuration.Persistence.Services;
  using Kritikos.Configuration.TestData.Model;

  using Microsoft.EntityFrameworkCore;

  using Xunit;

  public class AuditInterceptorTests : IClassFixture<DbContextFixture>
  {
    private static readonly Guid Creator = Guid.Parse("1813b30a-a352-416e-adee-282362f7ba4e");
    private static readonly Guid Editor = Guid.Parse("364b3527-0282-4fc7-aafc-547f2c87f641");

    private readonly DbContextFixture fixture;

    public AuditInterceptorTests(DbContextFixture fixture) => this.fixture = fixture;

    [Fact]
    public async Task CreatedBy_is_populated()
    {
      var records = DbContextFixture.GetEntries<AuditRecord>();

      await using var ctx = await fixture.GetContext(
        "createdBy",
        new AuditSaveChangesInterceptor<Guid>(new AuditService(() => Creator)));
      await ctx.Database.MigrateAsync();

      _ = records.Select(x => x.CreatedBy.Should().BeEmpty()).ToList();
      ctx.Records.AddRange(records);

      await ctx.SaveChangesAsync();
      _ = records.Select(x => x.CreatedBy.Should().Be(Creator)).ToList();
    }

    [Fact]
    public async Task UpdatedBy_changes_properly()
    {
      var records = DbContextFixture.GetEntries<AuditRecord>();

      await using var ctx = await fixture.GetContext(
        "updatedBy",
        new AuditSaveChangesInterceptor<Guid>(new AuditService(() => Creator)));
      await ctx.Database.MigrateAsync();

      _ = records.Select(x => x.UpdatedBy.Should().BeEmpty()).ToList();
      ctx.Records.AddRange(records);

      await ctx.SaveChangesAsync();
      _ = records.Select(x => x.UpdatedBy.Should().Be(Creator)).ToList();

      await using var editCtx = await fixture.GetContext(
        "updatedBy",
        new AuditSaveChangesInterceptor<Guid>(new AuditService(() => Editor)));

      _ = records = await editCtx.Records.ToListAsync();
      foreach (var rec in records)
      {
        rec.CreatedAt = DateTime.UtcNow;
      }

      await editCtx.SaveChangesAsync();

      _ = records.Select(x => x.UpdatedBy.Should().Be(Editor)).ToList();
    }

    public class AuditService : IAuditorProvider<Guid>
    {
      private readonly Func<Guid> fetchAuditor;

      public AuditService(Func<Guid> fetchAuditor) => this.fetchAuditor = fetchAuditor;

      /// <inheritdoc />
      public Guid GetAuditor() => fetchAuditor();

      /// <inheritdoc />
      public Guid GetFallbackAuditor() => Guid.Empty;
    }
  }
}
