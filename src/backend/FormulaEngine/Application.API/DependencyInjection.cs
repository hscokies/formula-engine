using System.Reflection;
using Application.API.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Application.API;

public static class DependencyInjection
{
    public static IServiceCollection AddRequestHandlers(this IServiceCollection services)
    {
        var domainTypes = new HashSet<Type>
        {
            typeof(ICommandHandler),
            typeof(IQueryHandler)
        };

        var handlers = typeof(DependencyInjection).Assembly.GetTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                           type.GetTypeInfo().ImplementedInterfaces.Intersect(domainTypes).Any());

        foreach (var handler in handlers)
        {
            services.AddScoped(handler);
        }
        
        return services;
    }
}
