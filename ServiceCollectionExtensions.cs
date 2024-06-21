using Microsoft.Extensions.DependencyInjection;

namespace IPeople.Roadrunner.Razor
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStateServices(this IServiceCollection services)
        {
            // Register any services required by your NuGet package components
            // Example:
            // services.AddSingleton<YourService>();

            services.AddSingleton<Services.RrStateService>();
            return services;
        }
    }
}
