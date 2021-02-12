#nullable disable
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
		public TKey Id { get; set; }

		/// <summary>
		/// Field used to track database concurrency hits.
		/// </summary>
		[Timestamp]
		public byte[] RowVersion { get; set; } = Array.Empty<byte>();
	}
}
