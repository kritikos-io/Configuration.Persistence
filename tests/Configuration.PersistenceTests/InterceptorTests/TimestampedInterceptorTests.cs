namespace Kritikos.Configuration.PersistenceTests.InterceptorTests
{
  using System;
  using System.Linq;
  using System.Threading.Tasks;

  using FluentAssertions;

  using Kritikos.Configuration.TestData.Model;

  using Microsoft.EntityFrameworkCore;

  using Xunit;

  public class TimestampedInterceptorTests : IClassFixture<DbContextFixture>
  {
    private readonly DbContextFixture fixture;
    private const int Count = 10;
    public TimestampedInterceptorTests(DbContextFixture fixture) => this.fixture = fixture;

    [Fact]
    public async Task ITimestamped_is_populated()
    {
      var records = DbContextFixture.GetEntries<AuditRecord>(Count);

      await using var ctx = await fixture.GetContext("createdAt", DbContextFixture.TimestampInterceptor);
      await ctx.Database.MigrateAsync();
      ctx.Records.AddRange(records);

      var before = DateTimeOffset.UtcNow;

      await ctx.SaveChangesAsync();

      var after = DateTimeOffset.UtcNow;

      _ = records.Select(x => x.CreatedAt.Should().BeAfter(before)).ToList();
      _ = records.Select(x => x.UpdatedAt.Should().BeBefore(after)).ToList();
    }

    [Fact]
    public async Task ITimestamped_UpdatedAt_is_changed()
    {
      await using var ctx = await fixture.GetContext("updatedAt", DbContextFixture.TimestampInterceptor);
      await ctx.Database.MigrateAsync();

      var records = DbContextFixture.GetEntries<AuditRecord>(Count);
      ctx.AddRange(records);

      await ctx.SaveChangesAsync();

      foreach (var auditRecord in records)
      {
        auditRecord.CreatedBy = new Guid();
        auditRecord.UpdatedBy = new Guid();
      }

      var (createdAt, updatedAt) = records.Select(x => (x.CreatedAt, x.UpdatedAt)).First();
      createdAt.Should().Be(updatedAt);

      await ctx.SaveChangesAsync();

      _ = records.Select(x => x.UpdatedAt.Should().BeAfter(createdAt)).ToList();
    }
  }
}
