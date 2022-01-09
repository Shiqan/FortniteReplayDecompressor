using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Unreal.Core.Contracts;

namespace Unreal.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNetFieldExportGroup(this IServiceCollection services, IServiceProvider serviceProvider, Type type)
    {
        var netfieldParser = serviceProvider.GetRequiredService<INetFieldParser>();
        netfieldParser.RegisterType(type);

        return services;
    }

    public static IServiceCollection AddNetFieldExportGroup<T>(this IServiceCollection services, IServiceProvider serviceProvider) where T : class, INetFieldExportGroup => services.AddNetFieldExportGroup(serviceProvider, typeof(T));

    public static IServiceCollection AddNetFieldExportGroups(this IServiceCollection services, IServiceProvider serviceProvider, IEnumerable<Type> netFieldExportGroups)
    {
        foreach (var netFieldExportGroup in netFieldExportGroups)
        {
            services.AddNetFieldExportGroup(serviceProvider, netFieldExportGroup);
        }
        return services;
    }

    public static IServiceCollection AddNetFieldExportGroupsFrom<TMarker>(this IServiceCollection services, IServiceProvider serviceProvider) => services.AddNetFieldExportGroupsFromAssembly(serviceProvider, typeof(TMarker).Assembly);

    public static IServiceCollection AddNetFieldExportGroupsFromAssembly(this IServiceCollection services, IServiceProvider serviceProvider, Assembly assembly) => services.AddNetFieldExportGroups(serviceProvider, assembly.GetTypes());
}
