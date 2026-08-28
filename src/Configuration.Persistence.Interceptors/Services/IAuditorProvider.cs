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
  /// <summary>
  /// Retrieves the current auditor, or <see langword="null"/> when none can be resolved.
  /// </summary>
  /// <returns>The auditor responsible for the change being persisted.</returns>
  T? GetAuditor();

  /// <summary>
  /// Retrieves the auditor to attribute a change to when <see cref="GetAuditor"/> yields nothing.
  /// </summary>
  /// <returns>The fallback auditor, typically representing the system itself.</returns>
  T GetFallbackAuditor();
}
