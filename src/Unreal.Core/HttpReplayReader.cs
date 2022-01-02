using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading;
using Unreal.Core.Exceptions;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace Unreal.Core;

/// <summary>
/// Abstract base class implementation of default UE HttpNetworkReplayStreaming.<br/>
/// <see href="https://github.com/EpicGames/UnrealEngine/blob/release/Engine/Source/Runtime/NetworkReplayStreaming/HttpNetworkReplayStreaming/Private/HttpNetworkReplayStreaming.cpp"/>
/// </summary>
public abstract class HttpReplayReader<T> : ReplayReader<T> where T : Replay, new()
{
    private readonly HttpClient _httpClient;

    protected HttpReplayReader(HttpClient httpClient, ILogger logger, ParseMode mode) : base(logger, mode)
    {
        _httpClient = httpClient;
    }

    [Obsolete("Not supported for HttpReplayReader<T>", true)]
    public override T ReadReplay(FArchive archive) => base.ReadReplay(archive);

    protected JsonSerializerOptions jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    /// <summary>
    /// <see href="https://docs.unrealengine.com/4.27/en-US/TestingAndOptimization/ReplaySystem/Streamers/HTTPRESTAPI/"/>
    /// </summary>
    public async Task<T> ReadReplay(string replayId, ReplayVersionHistory replayVersion = default, CancellationToken cancellationToken = default)
    {
        Replay = new T();

        var result = await _httpClient.PostAsync($"{replayId}/startDownloading?user=76561197991255957", null, cancellationToken);
        if (!result.IsSuccessStatusCode)
        {
            _logger?.LogError("Download request for replay {replayId} did not succeed.", replayId);
            throw new InvalidReplayException($"Download request for replay {replayId} did not succeed.");
        }

        using var contentStream = await result.Content.ReadAsStreamAsync(cancellationToken);
        var replay = await JsonSerializer.DeserializeAsync<FNetworkReplayStartDownloadingResponse>(contentStream, jsonSerializerOptions, cancellationToken);

        if (replay is null)
        {
            _logger?.LogError("The response of the download request for replay {replayId} could not be deserialized.", replayId);
            throw new InvalidReplayException($"The response of the download request for replay {replayId} could not be deserialized.");
        }

        if (replay.State != "Final")
        {
            throw new InvalidReplayException("The requested replay is not yet marked as final. Live streaming is not supported.");
        }

        // read header
        await DownloadAndParseHeader(replayId, replayVersion, cancellationToken);

        // read chunks
        await DownloadAndParseChunks(replayId, replay.NumChunks, replayVersion, cancellationToken);

        Cleanup();

        return Replay;
    }

    protected async Task DownloadAndParseHeader(string replayId, ReplayVersionHistory replayVersion, CancellationToken cancellationToken)
    {
        var bytes = await _httpClient.GetByteArrayAsync($"{replayId}/file/replay.header", cancellationToken);
        using var archive = new Unreal.Core.BinaryReader(bytes.AsMemory());
        archive.ReplayVersion = replayVersion;
        ReadReplayHeader(archive);
    }

    protected async Task DownloadAndParseChunks(string replayId, int chunkSize, ReplayVersionHistory replayVersion,CancellationToken cancellationToken) {
        for (var i = 0; i < chunkSize; i++)
        {
            var response = await _httpClient.GetByteArrayAsync($"{replayId}/file/stream.{i}", cancellationToken);
            using var chunkArchive = new Unreal.Core.BinaryReader(response.AsMemory());
            chunkArchive.ReplayVersion = replayVersion;
            chunkArchive.EngineNetworkVersion = Replay.Header.EngineNetworkVersion;
            chunkArchive.NetworkVersion = Replay.Header.NetworkVersion;
            chunkArchive.ReplayHeaderFlags = Replay.Header.Flags;
            ReadReplayData(chunkArchive, response.Length);
        }
    }
}
