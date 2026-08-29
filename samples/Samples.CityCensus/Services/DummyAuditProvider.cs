namespace Kritikos.Samples.CityCensus.Services;

using System;

using Kritikos.Configuration.Persistence.Interceptors.Services;

public class DummyAuditProvider(Func<Guid> fetchAuditor)
  : IAuditorProvider<Guid>
{
  private readonly Func<Guid> fetchAuditor = fetchAuditor;

  /// <inheritdoc />
  public Guid GetAuditor() => fetchAuditor();

  /// <inheritdoc />
  public Guid GetFallbackAuditor() => Guid.Empty;
}
