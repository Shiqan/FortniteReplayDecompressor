namespace Unreal.Core.Models;

/// <summary>
/// The response for downloading a replay from the replay server based on the HTTP Streamer. <br/>
/// <see href="https://docs.unrealengine.com/4.27/en-US/TestingAndOptimization/ReplaySystem/Streamers/HTTPRESTAPI/#requeststartdownload"/>
/// </summary>
public record FNetworkReplayStartDownloadingResponse
{
    public string? State { get; set; }
    public int NumChunks { get; set; }
    public int Time { get; set; }
    public string? ViewerId { get; set; }
}