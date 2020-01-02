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
	[SuppressMessage("Performance", "CA1819", Justification = "Proposed way to handle database concurrency")]
	[SuppressMessage("Performance", "CS8618", Justification = "Handled by database")]
	public abstract class Entity<TKey> : IEntity<TKey>, IConcurrent, ITimestamped, IAuditable
		where TKey : IComparable
	{
		/// <summary>
		/// Primary Key of entity.
		/// </summary>
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public TKey Id { get; set; }

		/// <summary>
		/// Field used to track database concurrency hits.
		/// </summary>
		[Timestamp]
		public byte[] RowVersion { get; set; }

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
