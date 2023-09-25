#pragma warning disable SA1402
namespace Kritikos.Configuration.Persistence.Contracts;

using Kritikos.Configuration.Persistence.Extensions;

/// <summary>
/// A simple interface to mark entities that are concurrently updated.
/// </summary>
public interface IConcurrent
{
}

/// <summary>
/// An interface exposing a row version for concurrent updates on Microsoft Sql Server.
/// </summary>
public interface ISqlServerConcurrent : IConcurrent
{
  byte[] RowVersion { get; set; }
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

/// <summary>
/// An interface exposing a row version for concurrent updates on PostgreSql Server.
/// </summary>
public interface IPostgreSqlConcurrent : IConcurrent
{
  public uint RowVersion { get; set; }
}
