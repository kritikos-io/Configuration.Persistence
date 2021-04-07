namespace Kritikos.Configuration.Persistence.HealthCheck
{
  public static class DbContextHealthLogTemplates
  {
    // Debug
    public const string AttemptingToConnect = "Initiating connection to {Database}";

    public const string CheckingForMigrations = "Checking if {Database} has pending migrations";

    // Information
    public const string DatabaseConnected = "Successfully Connected to {Database}";

    public const string DatabaseMigrated = "All migrations are sucessfully applied to {Database}";

    // Warning
    public const string PendingMigrations = "{Database} has {Count} pending migrations: {Migrations}";

    // Error
    public const string ConnectionFailed = "Failed to connect to {Database}";

    // Critical
    public const string UnknownError = "Unhandled exception while checking {Database} health";
  }
}
