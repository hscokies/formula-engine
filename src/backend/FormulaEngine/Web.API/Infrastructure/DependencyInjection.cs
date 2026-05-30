using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Web.API.Endpoints;

namespace Web.API.Infrastructure;

internal static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddEndpoints()
        {
            var serviceDescriptors = typeof(IEndpoint).Assembly.GetTypes()
                .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                               type.IsAssignableTo(typeof(IEndpoint)))
                .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
                .ToArray();

            services.TryAddEnumerable(serviceDescriptors);

            return services;
        }

        internal IServiceCollection AddSwagger()
        {
            services.AddEndpointsApiExplorer();
            return services.AddOpenApiDocument(settings =>
            {
                settings.Title = "Dungeon editor API";
                settings.Version = "v1";
            });
        }

        internal IServiceCollection ConfigureSerializerOptions()
        {
            return services.ConfigureHttpJsonOptions(x =>
            {
                x.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                x.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                x.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });
        }
    }

    internal static IApplicationBuilder MapEndpoints(
        this WebApplication app,
        RouteGroupBuilder? routeGroupBuilder = null)
    {
        var endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

        IEndpointRouteBuilder builder = routeGroupBuilder is null ? app : routeGroupBuilder;

        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoint(builder);
        }

        return app;
    }

    internal static IApplicationBuilder UseSwagger(this IApplicationBuilder app)
    {
        app.UseOpenApi(c => { c.Path = "/api/swagger/{documentName}/swagger.json"; });
        app.UseSwaggerUi(c =>
        {
            c.DocumentPath = "/api/swagger/{documentName}/swagger.json";
            c.Path = "/api/swagger";
        });
        
        return app;
    }
}
