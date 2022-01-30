using FortniteReplayReader.Options;
using Microsoft.Extensions.DependencyInjection;
using Unreal.Core;
using Unreal.Core.Contracts;
using Unreal.Core.Extensions;

namespace FortniteReplayReader.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseFortniteReplayReader(this IServiceCollection services, Action<FortniteReplayOptions>? configureOptions = default)
    {
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }

        services.AddSingleton<INetGuidCache, NetGuidCache>();
        services.AddSingleton<INetFieldParser, NetFieldParser>();
        services.AddSingleton<ReplayReader>();

        var provider = services.BuildServiceProvider();
        services.AddNetFieldExportGroupsFrom<ReplayReader>(provider);

        return services;
    }
}

