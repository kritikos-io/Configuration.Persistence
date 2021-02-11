#nullable disable
namespace Kritikos.Configuration.Peristence.IdentityServer
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;

	using IdentityServer4.EntityFramework.Entities;
	using IdentityServer4.EntityFramework.Extensions;
	using IdentityServer4.EntityFramework.Interfaces;
	using IdentityServer4.EntityFramework.Options;

	using Kritikos.Configuration.Persistence.Abstractions;

	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Infrastructure;
	using Microsoft.Extensions.Options;

	/// <summary>
	/// Database abstraction for a combined <see cref="DbContext"/> using ASP.NET Identity and Identity Server.
	/// </summary>
	/// <typeparam name="TUser">The type of user objects.</typeparam>
	/// <typeparam name="TRole">The type of role objects.</typeparam>
	/// <typeparam name="TKey">The type of the primary key for users and roles.</typeparam>
	/// <seealso cref="DbContext"/>
	/// <seealso cref="IPersistedGrantDbContext"/>
	/// <seealso cref="IConfigurationDbContext"/>
	/// <exception cref="InvalidOperationException">Could not resolve an instance of <seealso cref="ConfigurationStoreOptions"/>.</exception>
	/// <exception cref="InvalidOperationException">Could not resolve an instance of <seealso cref="OperationalStoreOptions"/>.</exception>
	public abstract class ApiAuthorizationPooledDbContext<TUser, TRole, TKey> :
		IdentityDbContext<TUser, TRole, TKey>,
		IPersistedGrantDbContext,
		IConfigurationDbContext
		where TUser : IdentityUser<TKey>, IEntity<TKey>
		where TRole : IdentityRole<TKey>, IEntity<TKey>
		where TKey : IComparable, IComparable<TKey>, IEquatable<TKey>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ApiAuthorizationDbContext{TUser,TRole,TKey}"/> class.
		/// </summary>
		/// <param name="options">The <see cref="DbContextOptions"/>.</param>
		protected ApiAuthorizationPooledDbContext(
			DbContextOptions options)
			: base(options)
		{
		}

		#region Implementation of IConfigurationDbContext

		/// <summary>
		/// Gets or sets the clients.
		/// </summary>
		/// <value>
		/// The clients.
		/// </value>
		public DbSet<Client> Clients { get; set; }

		/// <summary>
		/// Gets or sets the clients' CORS origins.
		/// </summary>
		/// <value>
		/// The clients CORS origins.
		/// </value>
		public DbSet<ClientCorsOrigin> ClientCorsOrigins { get; set; }

		/// <summary>
		/// Gets or sets the identity resources.
		/// </summary>
		/// <value>
		/// The identity resources.
		/// </value>
		public DbSet<IdentityResource> IdentityResources { get; set; }

		/// <summary>
		/// Gets or sets the API resources.
		/// </summary>
		/// <value>
		/// The API resources.
		/// </value>
		public DbSet<ApiResource> ApiResources { get; set; }

		/// <summary>
		/// Gets or sets the API scopes.
		/// </summary>
		/// <value>
		/// The API resources.
		/// </value>
		public DbSet<ApiScope> ApiScopes { get; set; }

		#endregion Implementation of IConfigurationDbContext

		#region Implementation of IPersistedGrantDbContext

		/// <summary>
		/// Gets or sets the persisted grants.
		/// </summary>
		/// <value>
		/// The persisted grants.
		/// </value>
		public DbSet<PersistedGrant> PersistedGrants { get; set; }

		/// <summary>
		/// Gets or sets the device codes.
		/// </summary>
		/// <value>
		/// The device codes.
		/// </value>
		public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }

		/// <inheritdoc cref="DbContext.SaveChangesAsync(CancellationToken)"/>
		public Task<int> SaveChangesAsync() => base.SaveChangesAsync();

		#endregion Implementation of IPersistedGrantDbContext

		#region Overrides of IdentityDbContext<TUser,TRole,TKey,IdentityUserClaim<TKey>,IdentityUserRole<TKey>,IdentityUserLogin<TKey>,IdentityRoleClaim<TKey>,IdentityUserToken<TKey>>

		/// <summary>
		/// Override this method to further configure the model that was discovered by convention from the entity types
		/// exposed in <see cref="T:Microsoft.EntityFrameworkCore.DbSet`1" /> properties on your derived context. The resulting model may be cached
		/// and re-used for subsequent instances of your derived context.
		/// </summary>
		/// /// <param name="modelBuilder">The builder being used to construct the model for this context. Databases (and other extensions) typically
		/// define extension methods on this object that allow you to configure aspects of the model that are specific
		/// to a given database.</param>
		/// <remarks>
		/// /// If a model is explicitly set on the options for this context (via <see cref="M:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)" />)
		/// then this method will not be run.
		/// </remarks>
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			var configurationStoreOptions = this.GetService<IOptions<ConfigurationStoreOptions>>()?.Value
																			?? throw new InvalidOperationException(nameof(ConfigurationStoreOptions));
			var operationalStoreOptions = this.GetService<IOptions<OperationalStoreOptions>>()?.Value
																		?? throw new InvalidOperationException(nameof(OperationalStoreOptions));

			modelBuilder.ConfigurePersistedGrantContext(operationalStoreOptions);
			modelBuilder.ConfigureResourcesContext(configurationStoreOptions);
			modelBuilder.ConfigureClientContext(configurationStoreOptions);
		}

		#endregion Overrides of IdentityDbContext<TUser,TRole,TKey,IdentityUserClaim<TKey>,IdentityUserRole<TKey>,IdentityUserLogin<TKey>,IdentityRoleClaim<TKey>,IdentityUserToken<TKey>>
	}
}
