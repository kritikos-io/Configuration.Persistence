namespace Kritikos.Configuration.Persistence.Services
{
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;
	using Microsoft.Extensions.Logging;

	public class MigrationService<TContext> : IHostedService
		where TContext : DbContext
	{
		private readonly IServiceScopeFactory scopeFactory;
		private readonly ILogger<MigrationService<TContext>> logger;
		private readonly string contextName = typeof(TContext).Name;

		public MigrationService(IServiceScopeFactory scopeFactory, ILogger<MigrationService<TContext>> logger)
		{
			this.scopeFactory = scopeFactory;
			this.logger = logger;
		}

		#region Implementation of IHostedService

		/// <inheritdoc />
		public async Task StartAsync(CancellationToken cancellationToken)
		{
			using var scope = scopeFactory.CreateScope();
			var ctx = scope.ServiceProvider.GetRequiredService<TContext>();
			var migrations = (await ctx.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();
			if (migrations.Any())
			{
				logger.LogInformation("Applying migrations to {DbContext}: {Migrations}", contextName, migrations);
				await ctx.Database.MigrateAsync(cancellationToken);
			}
		}

		/// <inheritdoc />
		public Task StopAsync(CancellationToken cancellationToken)
		{
			logger.LogInformation("Migrations for {DbContext} completed succesfully", contextName);
			return Task.CompletedTask;
		}

		#endregion
	}
}
