// ReSharper disable RedundantTypeDeclarationBody

#pragma warning disable SA1402
namespace Kritikos.Configuration.Persistence.Contracts;

using Kritikos.Configuration.Persistence.Extensions;

#pragma warning disable CA1040
/// <summary>
/// A simple interface to mark entities that are concurrently updated.
/// </summary>
public interface IConcurrent
{
}

/// <summary>
/// An interface hiding the row version for concurrent updates on PostgreSql Server.
/// </summary>
/// <remarks>
/// Use this with <see cref="ModelBuilderExtensions.EntitiesImplementing{T}"/> and an action registering
/// the UseXminAsConcurrencyToken().
/// </remarks>
[Obsolete($"Use {nameof(IPostgreSqlConcurrent)} instead.", false)]

public interface IPostgreSqlShadowConcurrent : IConcurrent
{
}
#pragma warning restore CA1040

/// <summary>
/// An interface exposing a row version for concurrent updates on PostgreSql Server.
/// </summary>
public interface IPostgreSqlConcurrent : IConcurrent
{
  public uint RowVersion { get; set; }
}

/// <summary>
/// An interface exposing a row version for concurrent updates on Microsoft Sql Server.
/// </summary>
public interface ISqlServerConcurrent : IConcurrent
{
#pragma warning disable CA1819 Defined as per the spec for optimistic concurrency
  byte[] RowVersion { get; set; }
#pragma warning restore CA1819
}
