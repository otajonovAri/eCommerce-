using EC.SharedLibrary.DI;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Application.Interfaces;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;

namespace ProductApi.Infrastructure.DI;

public static class ServiceContainer
{
    public static IServiceCollection AddInfrastructureService(this IServiceCollection services , IConfiguration configuration)
    {
        // Add DataBase Connection
        // Add Authentication scheme

        services.AddSharedServices<ProductDbContext>(configuration,
            configuration["MySerilog:FiLeName"]!);
        
        // Create Dependency Injection (DI)
        services.AddScoped<IProduct, ProductRepository>();
        
        return services;
    }

    public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
    {
        // Register Middleware such as:
        // Global Exception: Handles external errors :)
        // Listen To ONLY Api Gateway: blocks all outsider calls;

        app.UseSharedPolicies();
        
        return app;
    }
}
