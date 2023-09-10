using System.IO;
using Unreal.Core.Models;
using Unreal.Core.Test.Mocks;
using Xunit;

namespace Unreal.Core.Test;

public class ReadReplayDataTest
{
    [Theory]
    [InlineData(new byte[] { }, ReplayVersionHistory.HISTORY_INITIAL)]
    [InlineData(new byte[] {
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
    }, ReplayVersionHistory.HISTORY_FRIENDLY_NAME_ENCODING)]
    [InlineData(new byte[] {
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00
    }, ReplayVersionHistory.HISTORY_ENCRYPTION)]
    public void ReadReplayHeader(byte[] rawData, ReplayVersionHistory replayVersionHistory)
    {
        using var stream = new MemoryStream(rawData);
        using var archive = new Unreal.Core.BinaryReader(stream)
        {
            ReplayVersion = replayVersionHistory,
        };
        var reader = new MockReplayReader();
        reader.SetReplay(new MockReplay
        {
            Info = new ReplayInfo
            {
                IsEncrypted = false,
                IsCompressed = false,
            }
        });
        reader.ReadReplayData(archive, 0);
        Assert.True(archive.AtEnd());
        Assert.False(archive.IsError);
    }
}
