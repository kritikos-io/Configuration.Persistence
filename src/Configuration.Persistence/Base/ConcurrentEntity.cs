namespace Kritikos.Configuration.Persistence.Base
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	using Kritikos.Configuration.Persistence.Abstractions;

	public abstract class ConcurrentEntity<TKey> : IEntity<TKey>, IConcurrent
		where TKey : IComparable, IComparable<TKey>, IEquatable<TKey>
	{
		/// <summary>
		/// Primary Key of entity.
		/// </summary>
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
#pragma warning disable CS8618 // Primary key should be handled by database.
		public TKey Id { get; set; }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

		/// <summary>
		/// Field used to track database concurrency hits.
		/// </summary>
		[Timestamp]
#pragma warning disable CA1819 // Proposed way to handle database concurrency in SQL Server.
		public byte[] RowVersion { get; set; } = Array.Empty<byte>();
#pragma warning restore CA1819 // Properties should not return arrays
	}
}
