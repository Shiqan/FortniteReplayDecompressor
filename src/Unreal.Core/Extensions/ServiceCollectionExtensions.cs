using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Unreal.Core.Contracts;

namespace Unreal.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNetFieldExportGroup<T>(this IServiceCollection services) where T : class, INetFieldExportGroup => services.AddNetFieldExportGroup(typeof(T));
    
    public static IServiceCollection AddNetFieldExportGroup(this IServiceCollection services, Type netFieldExportGroup)
    {
        return services;
    }        
    
    public static IServiceCollection AddNetFieldExportGroups(this IServiceCollection services, IEnumerable<Type> netFieldExportGroups)
    {
        return services;
    }    
    
    public static IServiceCollection AddNetFieldExportGroupsFrom<TMarker>(this IServiceCollection services) => services.AddNetFieldExportGroupsFromAssembly(typeof(TMarker).Assembly);

    public static IServiceCollection AddNetFieldExportGroupsFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        return services;
    }
}
