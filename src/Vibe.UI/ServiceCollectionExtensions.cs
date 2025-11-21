using Microsoft.Extensions.DependencyInjection;
using Vibe.UI.Configuration;
using Vibe.UI.Services.Dialog;
using Vibe.UI.Services.Theme;
using Vibe.UI.Services.Toast;

namespace Vibe.UI
{
    /// <summary>
    /// Extension methods for registering Vibe.UI services with dependency injection.
    /// </summary>
    public static class ServiceCollectionExtensions
    {


        /// <summary>
        /// Adds Vibe.UI services with theme configuration to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configureTheme">Action to configure theme options.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddVibeUI(this IServiceCollection services, Action<VibeThemeOptions>? configureTheme = null)
        {
            // Configure theme options
            VibeThemeOptions themeOptions = new();
            if (configureTheme is not null)
                configureTheme(themeOptions);

            // Register theme options and service
            services.AddSingleton(themeOptions);
            services.AddSingleton<IThemeService, ThemeService>();

            // Register toast and dialog services
            services.AddScoped<IToastService, ToastService>();
            services.AddScoped<IDialogService, DialogService>();

            return services;
        }
    }
}