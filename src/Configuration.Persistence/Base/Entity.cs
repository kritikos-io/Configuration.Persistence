namespace Kritikos.Configuration.Persistence.Base
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Diagnostics.CodeAnalysis;

	using Kritikos.Configuration.Persistence.Abstractions;

	/// <summary>
	/// Provides a sane starting point to describe persistence.
	/// </summary>
	/// <typeparam name="TKey">Type of primary key.</typeparam>
	/// <remarks>Created/Updated properties should be handled by Entity Framework, not business logic.</remarks>
	[ExcludeFromCodeCoverage]
	public abstract class Entity<TKey> : IEntity<TKey>, IConcurrent, ITimestamped, IAuditable<string>
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
		public byte[] RowVersion { get; set; } = Array.Empty<byte>();

		/// <summary>
		/// Date of creation.
		/// </summary>
		public DateTimeOffset CreatedAt { get; set; }

		/// <summary>
		/// Date of last change.
		/// </summary>
		public DateTimeOffset UpdatedAt { get; set; }

		/// <summary>
		/// User that created this entity.
		/// </summary>
		public string CreatedBy { get; set; } = string.Empty;

		/// <summary>
		/// User that last changed this entity.
		/// </summary>
		public string UpdatedBy { get; set; } = string.Empty;
	}
}
