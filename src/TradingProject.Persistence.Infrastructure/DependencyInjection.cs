using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TradingProject.Persistence.Application.Abstractions;
using TradingProject.Persistence.Infrastructure.Persistence;
using TradingProject.Persistence.Infrastructure.Services;
using TradingProject.Persistence.Infrastructure.Settings;

namespace TradingProject.Persistence.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        TradingConnectionSettings.BindSettingsToProperties(services, configuration);

        services.AddSingleton<ConnectionStringFactory>();

        services.AddDbContext<TradingDbContext>((sp, options) =>
        {
            var factory = sp.GetRequiredService<ConnectionStringFactory>();
            options.UseNpgsql(factory.ConnectionString);
        });

        services.AddScoped<IDatabaseService, DatabaseService>();
        services.AddScoped<ITradingDbContext, TradingDbContext>();
        
        return services;
    }
}
