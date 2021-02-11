namespace Kritikos.Configuration.Persistence.Extensions
{
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Diagnostics;
	using Microsoft.Extensions.Hosting;

	public static class DbContextOptionsBuilderExtensions
	{
		/// <summary>
		/// Enables logging and sensitive data exposure if configuring on a development environment, supresses those warnings from logs and prevents cascading deletions.
		/// </summary>
		/// <param name="builder">The builder to operate on.</param>
		/// <param name="environment">The environment to enable development specific options.</param>
		/// <returns>The same builder instance so that multiple calls can be chained.</returns>
		public static DbContextOptionsBuilder EnableCommonOptions(
			this DbContextOptionsBuilder builder,
			IHostEnvironment environment)
			=> builder
				.EnableDetailedErrors(environment.IsDevelopment())
				.EnableSensitiveDataLogging(environment.IsDevelopment())
				.ConfigureWarnings(warn => warn
					.Ignore(CoreEventId.SensitiveDataLoggingEnabledWarning)
					.Log(RelationalEventId.QueryPossibleUnintendedUseOfEqualsWarning)
					.Throw(CoreEventId.CascadeDelete, CoreEventId.CascadeDeleteOrphan));

		/// <summary>
		/// Enables logging and sensitive data exposure if configuring on a development environment, supresses those warnings from logs and prevents cascading deletions.
		/// </summary>
		/// <param name="builder">The builder to operate on.</param>
		/// <param name="isDevelopment">If used on a development environment, suppresses warnings for sensitive data.</param>
		/// <returns>The same builder instance so that multiple calls can be chained.</returns>
		public static DbContextOptionsBuilder EnableCommonOptions(
			this DbContextOptionsBuilder builder,
			bool isDevelopment)
			=> builder
				.EnableDetailedErrors(isDevelopment)
				.EnableSensitiveDataLogging(isDevelopment)
				.ConfigureWarnings(warn => warn
					.Ignore(CoreEventId.SensitiveDataLoggingEnabledWarning)
					.Log(RelationalEventId.QueryPossibleUnintendedUseOfEqualsWarning)
					.Throw(CoreEventId.CascadeDelete, CoreEventId.CascadeDeleteOrphan));
	}
}
