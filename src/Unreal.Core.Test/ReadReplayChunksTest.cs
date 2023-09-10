using System.IO;
using Unreal.Core.Models.Enums;
using Unreal.Core.Test.Mocks;
using Xunit;

namespace Unreal.Core.Test;

public class ReadReplayChunksTest
{
    [Fact]
    public void ReadChunkShouldSeekEndOfChunkTest()
    {
        using var stream = new MemoryStream(new byte[] {
            0x09, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00
        });
        using var archive = new Unreal.Core.BinaryReader(stream);
        var reader = new MockReplayReader();

        reader.ReadReplayChunks(archive);
        Assert.True(archive.AtEnd());
        Assert.False(archive.IsError);
    }

    [Fact]
    public void ReplayDataChunkShouldBeSkippedTest()
    {
        using var stream = new MemoryStream(new byte[] {
            0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00
        });
        using var archive = new Unreal.Core.BinaryReader(stream);
        var reader = new MockReplayReader(parseMode: ParseMode.EventsOnly);

        reader.ReadReplayChunks(archive);
        Assert.True(archive.AtEnd());
        Assert.False(archive.IsError);
    }
}
