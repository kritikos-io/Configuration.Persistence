namespace Kritikos.Configuration.Persistence.Abstractions
{
	/// <summary>
	/// Exposes barebones auditing functionality on a multi-user system.
	/// </summary>
	public interface IAuditable
	{
		string CreatedBy { get; set; }

		string UpdatedBy { get; set; }
	}
}
