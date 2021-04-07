#nullable disable
namespace Kritikos.Configuration.Peristence.IdentityServer
{
  using System;
  using System.Diagnostics.CodeAnalysis;
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
  [ExcludeFromCodeCoverage]
  public abstract class ApiAuthorizationDbContext<TUser, TRole, TKey> :
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
    protected ApiAuthorizationDbContext(
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

    /// <inheritdoc cref="DbContext"/>
    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);
      var configurationStoreOptions = this.GetService<IOptions<ConfigurationStoreOptions>>()?.Value
                                      ?? throw new InvalidOperationException(nameof(ConfigurationStoreOptions));
      var operationalStoreOptions = this.GetService<IOptions<OperationalStoreOptions>>()?.Value
                                    ?? throw new InvalidOperationException(nameof(OperationalStoreOptions));

      builder.ConfigurePersistedGrantContext(operationalStoreOptions);
      builder.ConfigureResourcesContext(configurationStoreOptions);
      builder.ConfigureClientContext(configurationStoreOptions);
    }

    #endregion Overrides of IdentityDbContext<TUser,TRole,TKey,IdentityUserClaim<TKey>,IdentityUserRole<TKey>,IdentityUserLogin<TKey>,IdentityRoleClaim<TKey>,IdentityUserToken<TKey>>
  }
}
