namespace Kritikos.Configuration.Persistence.Extensions;

using System;
using System.Linq;
using System.Reflection;

using Kritikos.Configuration.Persistence.Contracts;
using Kritikos.Configuration.Persistence.Contracts.Behavioral;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public static class ModelBuilderExtensions
{
  /// <summary>
  /// Applies configuration to all entities implementing <see cref="IConfigurableEntity"/>.
  /// <remarks>This extension method works both on public and internal implementations of the interface.</remarks>
  /// </summary>
  /// <param name="modelBuilder">The builder being used to construct the model for this context.
  /// Databases (and other extensions) typically define extension methods on this object
  /// that allow you to configure aspects of the model that are specific to a given database.</param>
  public static void ApplyEntityConfiguration(this ModelBuilder modelBuilder)
  {
    ArgumentNullException.ThrowIfNull(modelBuilder);

    var eTypes = modelBuilder.Model.GetEntityTypes()
      .Where(x => typeof(IConfigurableEntity).IsAssignableFrom(x.ClrType))
      .Select(x => x.ClrType);

    foreach (var entityType in eTypes)
    {
      var methods = entityType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
      var method = methods.SingleOrDefault(x => x.Name == nameof(IConfigurableEntity.OnModelCreating));

      method?.Invoke(null, new object[] { modelBuilder });
    }
  }

  /// <summary>
  /// Allows configuring all instances inheriting the specified interface simultaneously.
  /// </summary>
  /// <typeparam name="T"><see langword="interface"/> to configure.</typeparam>
  /// <param name="modelBuilder"><seealso cref="ModelBuilder"/> instance to configure.</param>
  /// <param name="buildAction">The configuration action to be invoked for all instances inheriting <typeparamref name="T"/>.</param>
  /// <returns>The same <paramref name="modelBuilder"/> instance so that multiple calls can be chained.</returns>
  /// <exception cref="ArgumentNullException"><paramref name="modelBuilder"/> is null.</exception>
  /// <remarks>This overload is meant to be used with interfaces, if you have a base class prefer the strongly typed version <seealso cref="EntitiesOfType{T}(ModelBuilder,Action{EntityTypeBuilder{T}})"/>.</remarks>
  public static ModelBuilder EntitiesImplementing<T>(
    this ModelBuilder modelBuilder,
    Action<EntityTypeBuilder> buildAction)
  {
    ArgumentNullException.ThrowIfNull(modelBuilder);
    ArgumentNullException.ThrowIfNull(buildAction);

    var entityTypes = modelBuilder.Model.GetEntityTypes().Where(x => typeof(T).IsAssignableFrom(x.ClrType)).ToList();
    foreach (var entityType in entityTypes)
    {
      buildAction(modelBuilder.Entity(entityType.ClrType));
    }

    return modelBuilder;
  }

  /// <summary>
  /// Allows configuring all instances inheriting a base class simultaneously.
  /// </summary>
  /// <typeparam name="T"><see langword="interface"/> to configure.</typeparam>
  /// <param name="modelBuilder"><seealso cref="ModelBuilder"/> instance to configure.</param>
  /// <param name="buildAction">The configuration action to be invoked for all instances inheriting <typeparamref name="T"/>.</param>
  /// <returns>The same <paramref name="modelBuilder"/> instance so that multiple calls can be chained.</returns>
  /// <exception cref="ArgumentNullException"><paramref name="modelBuilder"/> is null.</exception>
  /// <remarks>This overload is meant to be used with classes, if you only have an interface use <seealso cref="EntitiesImplementing{T}"/>.</remarks>
  public static ModelBuilder EntitiesOfType<T>(
    this ModelBuilder modelBuilder,
    Action<EntityTypeBuilder<T>> buildAction)
    where T : class
  {
    ArgumentNullException.ThrowIfNull(modelBuilder);
    ArgumentNullException.ThrowIfNull(buildAction);

    var entityTypes = modelBuilder.Model.GetEntityTypes().Where(x => typeof(T).IsAssignableFrom(x.ClrType)).ToList();
    foreach (var unused in entityTypes)
    {
      buildAction(modelBuilder.Entity<T>());
    }

    return modelBuilder;
  }
}
