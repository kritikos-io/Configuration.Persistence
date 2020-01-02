namespace Kritikos.Configuration.Persistence.Base
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Diagnostics.CodeAnalysis;

	using Kritikos.Configuration.Persistence.Abstractions;

	[SuppressMessage("Performance", "CA1819", Justification = "Proposed way to handle database concurrency")]
	[SuppressMessage("Performance", "CS8618", Justification = "Handled by database")]
	public abstract class ConcurrentEntity<TKey> : IEntity<TKey>, IConcurrent
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
	}
}
