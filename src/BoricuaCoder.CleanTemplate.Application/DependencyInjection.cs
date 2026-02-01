using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BoricuaCoder.CleanTemplate.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddValidatorsFromAssembly(assembly);

        // Register all command and query handlers from this assembly
        var handlerInterfaces = new[] { typeof(Common.CQRS.ICommandHandler<,>), typeof(Common.CQRS.IQueryHandler<,>) };

        foreach (var handlerInterface in handlerInterfaces)
        {
            var implementations = assembly.GetTypes()
                .Where(t => t is { IsAbstract: false, IsInterface: false })
                .SelectMany(t => t.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterface)
                    .Select(i => new { Interface = i, Implementation = t }));

            foreach (var handler in implementations)
            {
                services.AddScoped(handler.Interface, handler.Implementation);
            }
        }

        return services;
    }
}
