using Microsoft.Extensions.DependencyInjection;
using Unreal.Core.Contracts;
using Unreal.Core.Options;

namespace Unreal.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUnrealCore(this IServiceCollection services, Action<NetFieldParserOptions>? netFieldParserOptions = default)
    {
        if (netFieldParserOptions != null)
        {
            services.Configure(netFieldParserOptions);
        }

        services.AddSingleton<INetGuidCache, NetGuidCache>();
        services.AddSingleton<INetFieldParser, NetFieldParser>();

        return services;
    }
}
