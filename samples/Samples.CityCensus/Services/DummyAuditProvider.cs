namespace Kritikos.Samples.CityCensus.Services;

using System;

using Kritikos.Configuration.Persistence.Interceptors.Services;

public class DummyAuditProvider(Func<Guid?> fetchAuditor, Guid fallback = default)
  : IAuditorProvider<Guid>
{
  private readonly Func<Guid?> fetchAuditor = fetchAuditor;

  /// <inheritdoc />
  public bool TryGetAuditor(out Guid auditor)
  {
    var fetched = fetchAuditor();
    auditor = fetched ?? default;

    return fetched.HasValue;
  }

  /// <inheritdoc />
  public Guid GetFallbackAuditor() => fallback;
}
