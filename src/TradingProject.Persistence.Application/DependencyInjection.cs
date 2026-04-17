using Microsoft.Extensions.DependencyInjection;

namespace TradingProject.Persistence.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}
