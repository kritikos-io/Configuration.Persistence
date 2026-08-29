namespace Kritikos.Configuration.Persistence.Extensions;

using System;

using Kritikos.Configuration.Persistence.Contracts.Behavioral;
using Kritikos.Configuration.Persistence.Entities;

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// Extension methods keeping individual properties of an <see cref="ITraceableAudit"/> entity out of the values recorded in its audit trail.
/// </summary>
public static class AuditTrailExtensions
{
  private const string Annotation = "Kritikos:ExcludedFromAuditTrail";

  /// <summary>
  /// Keeps the value of this property out of <see cref="AuditRecord.OldValues"/> and <see cref="AuditRecord.NewValues"/>.
  /// </summary>
  /// <param name="property">The builder being used to configure the property.</param>
  /// <param name="excluded">If true, the value is never written to the trail.</param>
  /// <typeparam name="TProperty">The type of the property being configured.</typeparam>
  /// <returns>The same builder instance so that multiple calls can be chained.</returns>
  /// <remarks>The property is still named in <see cref="AuditRecord.Redacted"/> whenever it changes, so the trail records that it moved without disclosing either value. Primary keys are never excluded, since a record that cannot be traced back to a row is worthless.</remarks>
  /// <exception cref="ArgumentNullException"><paramref name="property"/> is null.</exception>
  public static PropertyBuilder<TProperty> ExcludeFromAuditTrail<TProperty>(this PropertyBuilder<TProperty> property, bool excluded = true)
  {
    ArgumentNullException.ThrowIfNull(property);

    property.Metadata.SetAnnotation(Annotation, excluded);

    return property;
  }

  /// <summary>
  /// Keeps the value of this property out of <see cref="AuditRecord.OldValues"/> and <see cref="AuditRecord.NewValues"/>.
  /// </summary>
  /// <param name="property">The builder being used to configure the property, typically a shadow one.</param>
  /// <param name="excluded">If true, the value is never written to the trail.</param>
  /// <returns>The same builder instance so that multiple calls can be chained.</returns>
  /// <remarks>The property is still named in <see cref="AuditRecord.Redacted"/> whenever it changes, so the trail records that it moved without disclosing either value. Primary keys are never excluded, since a record that cannot be traced back to a row is worthless.</remarks>
  /// <exception cref="ArgumentNullException"><paramref name="property"/> is null.</exception>
  public static PropertyBuilder ExcludeFromAuditTrail(this PropertyBuilder property, bool excluded = true)
  {
    ArgumentNullException.ThrowIfNull(property);

    property.Metadata.SetAnnotation(Annotation, excluded);

    return property;
  }

  /// <summary>
  /// Gets a value indicating whether the value of this property is kept out of the audit trail.
  /// </summary>
  /// <param name="property">The property to inspect.</param>
  /// <returns>True if <see cref="ExcludeFromAuditTrail{TProperty}"/> was applied to the property.</returns>
  /// <exception cref="ArgumentNullException"><paramref name="property"/> is null.</exception>
  public static bool IsExcludedFromAuditTrail(this IReadOnlyProperty property)
  {
    ArgumentNullException.ThrowIfNull(property);

    return property.FindAnnotation(Annotation)?.Value is true;
  }
}
