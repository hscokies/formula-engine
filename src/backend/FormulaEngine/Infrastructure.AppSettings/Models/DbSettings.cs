namespace Infrastructure.AppSettings.Models;

public sealed class DbSettings : IAppSettings
{
    public required string ConnectionString { get; set; }
}
