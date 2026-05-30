using Infrastructure.AppSettings.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.AppSettings;

public static class DependencyInjection
{
    public static IConfiguration AddJsonConfigurations(this IConfigurationManager manager, string name = "appsettings")
    {
        manager
            .AddJsonFile($"{name}.json", false)
            .AddJsonFile($"{name}.local.json", true);

        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        if (!string.IsNullOrWhiteSpace(environment))
        {
            manager.AddJsonFile($"{name}.{environment}.json", true);
        }
        
        return manager;
    }
    
    public static IServiceCollection AddSettings<TSettings>(this IServiceCollection services,  IConfiguration configuration) where TSettings : class, IAppSettings
    {
        return services.Configure<TSettings>(configuration.GetSection(typeof(TSettings).Name));
    }
}