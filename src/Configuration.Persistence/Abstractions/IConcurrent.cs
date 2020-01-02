namespace Kritikos.Configuration.Persistence.Abstractions
{
	using System.Diagnostics.CodeAnalysis;

	/// <summary>
	/// Exposes tracking field to enforce database concurrency.
	/// </summary>
	[SuppressMessage("Performance", "CA1819", Justification = "Proposed way to handle database concurrency")]
	public interface IConcurrent
	{
		byte[] RowVersion { get; set; }
	}
}
