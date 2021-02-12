namespace Kritikos.Configuration.TestData.Model
{
	using System;

	using Kritikos.Configuration.Persistence.Abstractions;

	public class AuditRecord : IEntity<long>, ITimestamped, IAuditable<Guid>
	{
		public long Id { get; set; }

		public DateTimeOffset CreatedAt { get; set; }

		public DateTimeOffset UpdatedAt { get; set; }

		public Guid CreatedBy { get; set; }

		public Guid UpdatedBy { get; set; }
	}
}
