#nullable disable
namespace Kritikos.Samples.CityCensus
{
  using Kritikos.Configuration.Persistence.Contracts;
  using Kritikos.Configuration.Persistence.Entities;

  using Microsoft.EntityFrameworkCore;

  public class CityCensusTrailDbContext : DbContext, IAuditTrailDbContext<AuditRecord>
  {
    public CityCensusTrailDbContext()
    {
    }

    public CityCensusTrailDbContext(DbContextOptions<CityCensusTrailDbContext> options)
      : base(options)
    {
    }

    public DbSet<AuditRecord> AuditRecords { get; set; }

    public DbSet<County> Counties { get; set; }

    public DbSet<Corporation> Corporations { get; set; }

    public DbSet<CountyCorporation> CountyCorporations { get; set; }

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      if (!optionsBuilder.IsConfigured)
      {
        optionsBuilder.UseNpgsql("foobar");
      }

      base.OnConfiguring(optionsBuilder);
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
      AuditRecord.OnModelCreating(modelBuilder);
      County.OnModelCreating(modelBuilder);
      Corporation.OnModelCreating(modelBuilder);
      CountyCorporation.OnModelCreating(modelBuilder);
    }
  }
}
