namespace Kritikos.Configuration.Persistence.Interceptors.Services;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// A service definition that retrieves the auditor.
/// Recommended usage as a scoped service via Dependency Injection.
/// </summary>
/// <typeparam name="T">Type of audit field.</typeparam>
/// <remarks>
/// Absence is reported by the return value rather than by a null <typeparamref name="T"/>, because a value type such as <see cref="Guid"/> has no null to report it with and would leave <see cref="GetFallbackAuditor"/> unreachable.
/// </remarks>
public interface IAuditorProvider<T>
  where T : IComparable, IComparable<T>, IEquatable<T>
{
  /// <summary>
  /// Attempts to retrieve the current auditor.
  /// </summary>
  /// <param name="auditor">When this method returns <see langword="true"/>, the auditor responsible for the change being persisted, otherwise the default value of <typeparamref name="T"/>.</param>
  /// <returns><see langword="true"/> if an auditor could be resolved, otherwise <see langword="false"/>.</returns>
  bool TryGetAuditor([MaybeNullWhen(false)] out T auditor);

  /// <summary>
  /// Retrieves the auditor to attribute a change to when <see cref="TryGetAuditor"/> resolves nothing.
  /// </summary>
  /// <returns>The fallback auditor, typically representing the system itself.</returns>
  T GetFallbackAuditor();
}
