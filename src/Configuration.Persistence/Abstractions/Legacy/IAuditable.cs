namespace Kritikos.Configuration.Persistence.Abstractions
{
	using System;

	/// <summary>
	/// Exposes barebones auditing functionality on a multi-user system.
	/// </summary>
	[Obsolete("Replaced by IAuditable<string>.")]
	public interface IAuditable : IAuditable<string>
	{
	}
}
