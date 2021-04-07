namespace Kritikos.Configuration.Persistence.Extensions
{
  using System;
  using System.Linq;

  using Microsoft.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore.Metadata.Builders;

  public static class ModelBuilderExtensions
  {
    /// <summary>
    /// Allows configuring all instances inheriting the specified interface simultaneously.
    /// </summary>
    /// <typeparam name="T"><see langword="interface"/> to configure.</typeparam>
    /// <param name="modelBuilder"><seealso cref="ModelBuilder"/> instance to configure.</param>
    /// <param name="buildAction">The configuration action to be invoked for all instances inheriting <typeparamref name="T"/>.</param>
    /// <returns>The same <paramref name="modelBuilder"/> instance so that multiple calls can be chained.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="modelBuilder"/> is null.</exception>
    /// <remarks>This overload is meant to be used with interfaces, if you have a base class prefer the strongly typed version <seealso cref="EntitiesOfType{T}(ModelBuilder,Action{EntityTypeBuilder{T}})"/>.</remarks>
    public static ModelBuilder EntitiesOfType<T>(
      this ModelBuilder modelBuilder,
      Action<EntityTypeBuilder> buildAction)
    {
      if (modelBuilder == null)
      {
        throw new ArgumentNullException(nameof(modelBuilder));
      }

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
    /// <remarks>This overload is meant to be used with classes, if you only have an interface use <seealso cref="EntitiesOfType{T}(ModelBuilder,Action{EntityTypeBuilder})"/>.</remarks>
    public static ModelBuilder EntitiesOfType<T>(
      this ModelBuilder modelBuilder,
      Action<EntityTypeBuilder<T>> buildAction)
      where T : class
    {
      if (modelBuilder == null)
      {
        throw new ArgumentNullException(nameof(modelBuilder));
      }

      var entityTypes = modelBuilder.Model.GetEntityTypes().Where(x => typeof(T).IsAssignableFrom(x.ClrType)).ToList();
      foreach (var unused in entityTypes)
      {
        buildAction(modelBuilder.Entity<T>());
      }

      return modelBuilder;
    }
  }
}
