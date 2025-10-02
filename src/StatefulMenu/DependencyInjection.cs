using System.Globalization;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using StatefulMenu.Commands.Interfaces;
using StatefulMenu.Core.Interfaces;
using StatefulMenu.Infrastructure.Components;
using StatefulMenu.Infrastructure.Localization;
using StatefulMenu.Infrastructure.Services;

namespace StatefulMenu;

public static class DependencyInjection
{
    public static IServiceCollection AddStatefulMenu(this IServiceCollection services, params Assembly[] scanAssemblies)
    {
        services.AddSingleton<IConsoleLocalizer>(sp =>
        {
            var lang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            return lang == "ru" ? new RuConsoleLocalizer() : new EnConsoleLocalizer();
        });
        services.AddSingleton<NavigationStack>();
        services.AddSingleton<MenuRenderer>();
        services.AddSingleton<IDataService, DataService>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IConsoleInputService, ConsoleInputService>();

        var assemblies = scanAssemblies is {Length: > 0}
            ? scanAssemblies
            : new[] {Assembly.GetCallingAssembly()};

        foreach (var assembly in assemblies)
        {
            RegisterImplementations<IMenuProvider>(services, assembly, ServiceLifetime.Transient);
            RegisterImplementations<IMenuCommand>(services, assembly, ServiceLifetime.Transient);
        }

        return services;
    }

    private static void RegisterImplementations<TInterface>(IServiceCollection services, Assembly assembly,
        ServiceLifetime lifetime)
    {
        var interfaceType = typeof(TInterface);
        var types = assembly
            .GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface && interfaceType.IsAssignableFrom(t));

        foreach (var type in types)
        {
            services.Add(new ServiceDescriptor(type, type, lifetime));
            services.Add(new ServiceDescriptor(interfaceType, sp => sp.GetRequiredService(type), lifetime));
        }
    }
}