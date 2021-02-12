namespace Kritikos.Configuration.Persistence.Base
{
	using System;
	using System.Diagnostics.CodeAnalysis;

	using Kritikos.Configuration.Persistence.Abstractions;

	[ExcludeFromCodeCoverage]
	public abstract class BasicEntity<TKey> : IEntity<TKey>
		where TKey : IComparable, IComparable<TKey>, IEquatable<TKey>
	{
		/// <summary>
		/// Primary Key of entity.
		/// </summary>
#pragma warning disable CS8618 // Primary key should be handled by database.
		public TKey Id { get; set; }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
	}
}
