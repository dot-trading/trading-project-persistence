using Cortex.Mediator.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace TradingProject.Persistence.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddCortexMediator([typeof(DependencyInjection)]);
        services.AddAutoMapper(typeof(DependencyInjection));

        return services;
    }
}
