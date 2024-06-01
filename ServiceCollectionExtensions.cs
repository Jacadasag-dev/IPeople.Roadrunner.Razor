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

            services.AddSingleton<Shared.RrServices.ViewStateService>();
            services.AddSingleton<Shared.RrServices.InputStateService>();
            services.AddSingleton<Shared.RrServices.DropdownStateService>();
            services.AddSingleton<Shared.RrServices.PanelStateService>();
            services.AddSingleton<Shared.RrServices.ErrorLoggingService>();
            return services;
        }
    }
}
