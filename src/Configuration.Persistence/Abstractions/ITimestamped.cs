namespace Kritikos.Configuration.Persistence.Abstractions
{
	using System;

	/// <summary>
	/// Exposes timestamping functionality for barebones persistence auditing.
	/// </summary>
	public interface ITimestamped
	{
		DateTimeOffset CreatedAt { get; set; }

		DateTimeOffset UpdatedAt { get; set; }
	}
}
