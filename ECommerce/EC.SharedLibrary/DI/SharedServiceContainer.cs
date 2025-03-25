using EC.SharedLibrary.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace EC.SharedLibrary.DI;

public static class SharedServiceContainer
{
    public static IServiceCollection AddSharedServices<TContext>(this IServiceCollection services,
        IConfiguration configuration , string fileName) where TContext : DbContext
    {
        // Add Generic Database Context 
        services.AddDbContext<TContext>(options => options.UseNpgsql(
                configuration.GetConnectionString("eCommerceDb"),
                npgsqlOptions => npgsqlOptions.EnableRetryOnFailure()
            ));
        
        // configure serilog logging 
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Debug()
            .WriteTo.Console()
            .WriteTo.File(path: $"{fileName}-.text",
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day)
            .CreateLogger();
        
        // Add JWT authentication Scheme
        services.AddJwtAuthenticationScheme(configuration);
        return services;
    }

    public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
    {
        // Use Global Exception
        app.UseMiddleware<GlobalException>();
        
        // Register Middleware to block all outsiders API calls
        app.UseMiddleware<ListenToOnlyApiGateway>();
        
        return app;
    }
}
