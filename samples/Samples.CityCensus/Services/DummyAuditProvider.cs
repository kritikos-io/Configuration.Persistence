namespace Kritikos.Samples.CityCensus.Services
{
  using System;

  using Kritikos.Configuration.Persistence.Interceptors.Services;

  public class DummyAuditProvider : IAuditorProvider<Guid>
  {
    private readonly Func<Guid> fetchAuditor;

    public DummyAuditProvider(Func<Guid> fetchAuditor) => this.fetchAuditor = fetchAuditor;

    /// <inheritdoc />
    public Guid GetAuditor() => fetchAuditor();

    /// <inheritdoc />
    public Guid GetFallbackAuditor() => Guid.Empty;
  }
}
