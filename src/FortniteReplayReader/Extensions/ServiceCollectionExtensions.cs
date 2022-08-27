using FortniteReplayReader.Options;
using Microsoft.Extensions.DependencyInjection;
using Unreal.Core.Extensions;
using Unreal.Core.Options;

namespace FortniteReplayReader.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register required services.
    /// </summary>
    public static IServiceCollection UseFortniteReplayReader(this IServiceCollection services, Action<FortniteReplayOptions>? replayOptions = default, Action<NetFieldParserOptions>? netFieldParserOptions = default)
    {
        if (replayOptions != null)
        {
            services.Configure(replayOptions);
        }

        services.AddUnrealCore(netFieldParserOptions);
        services.AddSingleton<ReplayReader>();

        return services;
    }    
    
    /// <summary>
    /// Register required services, and automatically register all types for the given <typeparamref name="TMarker"/> assembly.
    /// </summary>
    public static IServiceCollection UseFortniteReplayReader<TMarker>(this IServiceCollection services, Action<FortniteReplayOptions>? replayOptions = default)
    {
        if (replayOptions != null)
        {
            services.Configure(replayOptions);
        }

        services.AddUnrealCore(netFieldParserOptions: o =>
        {
            o.AddNetFieldExportGroupsFromAssembly<TMarker>();
        });

        services.AddSingleton<ReplayReader>();

        return services;
    }
}

