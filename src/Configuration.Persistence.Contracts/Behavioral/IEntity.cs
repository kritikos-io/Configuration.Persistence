// ReSharper disable RedundantTypeDeclarationBody
// ReSharper disable UnusedTypeParameter

#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable CA1040 // Avoid empty interfaces
namespace Kritikos.Configuration.Persistence.Contracts.Behavioral;

using System;

/// <summary>
/// Marker interface used to decorate entities.
/// </summary>
public interface IEntity
{
}

/// <summary>
/// Exposes basic behavior for relational entity persistence.
/// </summary>
/// <typeparam name="TKey">Type of primary key.</typeparam>
public interface IEntity<TKey> : IEntity
  where TKey : IComparable<TKey>, IEquatable<TKey>
{
  TKey Id { get; set; }
}

/// <summary>
/// Marker interface to identify junction tables.
/// </summary>
public interface IJoinEntity : IEntity
{
}

/// <summary>
/// Typed version that identifies entities joined by an <see cref="IJoinEntity"/>
/// </summary>
/// <typeparam name="TLeft">The first side of the many-to-many relation being mapped.</typeparam>
/// <typeparam name="TRight">The second side of the many-to-many relation being mapped.</typeparam>
public interface IJoinEntity<TLeft, TRight> : IJoinEntity
  where TLeft : IEntity
  where TRight : IEntity
{
}

/// <summary>
/// Typed version that identifies entities joined by an <see cref="IJoinEntity"/> and strongly typed keys.
/// </summary>
/// <typeparam name="TLeft">The first side of the many-to-many relation being mapped.</typeparam>
/// <typeparam name="TKeyLeft">The type of the primary key used in <see cref="TLeft"/>.</typeparam>
/// <typeparam name="TRight">The second side of the many-to-many relation being mapped.</typeparam>
/// <typeparam name="TKeyRight">The type of the primary key used in <see cref="TRight"/>.</typeparam>
public interface IJoinEntity<TLeft, TKeyLeft, TRight, TKeyRight> : IJoinEntity<TLeft, TRight>
  where TLeft : IEntity<TKeyLeft>
  where TRight : IEntity<TKeyRight>
  where TKeyLeft : IComparable<TKeyLeft>, IEquatable<TKeyLeft>
  where TKeyRight : IComparable<TKeyRight>, IEquatable<TKeyRight>
{
}
