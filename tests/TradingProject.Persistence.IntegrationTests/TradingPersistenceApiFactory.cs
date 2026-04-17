using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Microsoft.EntityFrameworkCore;
using TradingProject.Persistence.Infrastructure.Persistence;

namespace TradingProject.Persistence.IntegrationTests;

public class TradingPersistenceApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:15-alpine")
        .WithDatabase("trading")
        .WithUsername("trading")
        .WithPassword("trading_secure_pwd_2026")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Inject the port and host for settings evaluation
        Environment.SetEnvironmentVariable("POSTGRES_HOST", _dbContainer.Hostname);
        Environment.SetEnvironmentVariable("POSTGRES_PORT", _dbContainer.GetMappedPublicPort(5432).ToString());
        Environment.SetEnvironmentVariable("POSTGRES_PASSWORD", "trading_secure_pwd_2026");

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TradingDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            services.AddDbContext<TradingDbContext>((sp, options) =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString());
            });

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TradingDbContext>();
            db.Database.EnsureCreated();
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}
