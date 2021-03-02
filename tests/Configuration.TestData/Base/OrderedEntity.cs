namespace Kritikos.Configuration.TestData.Base
{
	using System;

	public abstract class OrderedEntity<TKey> : TestEntity<TKey>
		where TKey : IComparable, IComparable<TKey>, IEquatable<TKey>
	{
		public Guid Order { get; set; }
	}
}
