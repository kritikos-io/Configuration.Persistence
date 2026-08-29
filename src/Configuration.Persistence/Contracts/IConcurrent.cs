// ReSharper disable RedundantTypeDeclarationBody

#pragma warning disable SA1402 // File may only contain a single type
namespace Kritikos.Configuration.Persistence.Contracts;

#pragma warning disable CA1040 // Avoid empty interfaces
/// <summary>
/// A simple interface to mark entities that are concurrently updated.
/// </summary>
public interface IConcurrent
{
}
#pragma warning restore CA1040

/// <summary>
/// An interface exposing a row version for concurrent updates on PostgreSql Server.
/// </summary>
public interface IPostgreSqlConcurrent : IConcurrent
{
  /// <summary>
  /// Gets or sets the PostgreSql <c>xmin</c> system column acting as the concurrency token.
  /// </summary>
  public uint RowVersion { get; set; }
}

/// <summary>
/// An interface exposing a row version for concurrent updates on Microsoft Sql Server.
/// </summary>
public interface ISqlServerConcurrent : IConcurrent
{
#pragma warning disable CA1819 // Defined as per the spec for optimistic concurrency
  /// <summary>
  /// Gets or sets the Sql Server <c>rowversion</c> column acting as the concurrency token.
  /// </summary>
  byte[] RowVersion { get; set; }
#pragma warning restore CA1819
}
