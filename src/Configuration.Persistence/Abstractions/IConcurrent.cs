namespace Kritikos.Configuration.Persistence.Abstractions
{
	using System.Diagnostics.CodeAnalysis;

	/// <summary>
	/// Exposes tracking field to enforce database concurrency.
	/// </summary>
	public interface IConcurrent
	{
		byte[] RowVersion { get; set; }
	}
}
