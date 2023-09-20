namespace Kritikos.Configuration.Persistence.Contracts;

using Kritikos.Configuration.Persistence.Contracts.Behavioral;
using Kritikos.Configuration.Persistence.Extensions;

using Microsoft.EntityFrameworkCore;

/// <summary>
/// Marks entities that can be configured via <see cref="ModelBuilder"/>.
/// </summary>
public interface IConfigurableEntity : IEntity
{
  /// <summary>
  /// Applies model configuration to the provided builder.
  /// Applied on <see cref="ModelBuilder"/> via <see cref="ModelBuilderExtensions.ApplyEntityConfiguration"/>.
  /// </summary>
  /// <remarks>Use by either defining a public method, or hide by creating an internal one.</remarks>
  /// <param name="builder">The builder being used to construct the model for this context.
  /// Databases (and other extensions) typically define extension methods on this object
  /// that allow you to configure aspects of the model that are specific to a given database.</param>
  public static virtual void OnModelCreating(ModelBuilder builder)
  {
  }
}
