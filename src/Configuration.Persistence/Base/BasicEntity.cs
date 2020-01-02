namespace Kritikos.Configuration.Persistence.Base
{
	using System;
	using System.Diagnostics.CodeAnalysis;

	using Kritikos.Configuration.Persistence.Abstractions;

	[SuppressMessage("Performance", "CS8618", Justification = "Handled by database")]
	public abstract class BasicEntity<TKey> : IEntity<TKey>
		where TKey : IComparable
	{
		/// <summary>
		/// Primary Key of entity.
		/// </summary>
		public TKey Id { get; set; }
	}
}
