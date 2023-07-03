namespace Kritikos.Configuration.Persistence.Interceptors.Services;

using System;

/// <summary>
/// A service definition that retrieves the auditor.
/// Recommended usage as a scoped service via Dependency Injection.
/// </summary>
/// <typeparam name="T">Type of audit field.</typeparam>
public interface IAuditorProvider<out T>
  where T : IComparable, IComparable<T>, IEquatable<T>
{
  T? GetAuditor();

  T GetFallbackAuditor();
}
