// ReSharper disable RedundantTypeDeclarationBody
// ReSharper disable UnusedTypeParameter
#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable CA1040 // Avoid empty interfaces
namespace Kritikos.Configuration.Persistence.Contracts.Behavioral;

using System;

/// <summary>
/// Marker interface used to decorate join entities.
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

public interface IJoinEntity : IEntity
{
}

public interface IJoinEntity<TLeft, TRight> : IJoinEntity
  where TLeft : IEntity
  where TRight : IEntity
{
}

public interface IJoinEntity<TLeft, TKeyLeft, TRight, TKeyRight> : IJoinEntity<TLeft, TRight>
  where TLeft : IEntity<TKeyLeft>
  where TRight : IEntity<TKeyRight>
  where TKeyLeft : IComparable<TKeyLeft>, IEquatable<TKeyLeft>
  where TKeyRight : IComparable<TKeyRight>, IEquatable<TKeyRight>
{
}
