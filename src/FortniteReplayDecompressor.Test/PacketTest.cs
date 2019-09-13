using FortniteReplayReader.Core.Models;
using FortniteReplayReader.Core.Models.Enums;
using System.IO;
using Xunit;

namespace FortniteReplayDecompressor.Test
{
    public class PacketTest
    {
        [Fact]
        public void ProcessPacketTest()
        {
            var packet = @"Packets/packet0.dump";
            using (var stream = File.Open(packet, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var replay = new Replay()
                {
                    Header = new Header()
                    {
                        EngineNetworkVersion = EngineNetworkVersionHistory.HISTORY_NEW_ACTOR_OVERRIDE_LEVEL
                    }
                };

                using (var reader = new FortniteBinaryDecompressor(stream, replay))
                {
                    using (var ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        var playbackPacket = new PlaybackPacket()
                        {
                            Data = ms.ToArray()
                        };
                        reader.ReceivedRawPacket(playbackPacket);
                    }
                    Assert.Equal(reader.BaseStream.Length, reader.BaseStream.Position);
                }

            }
        }

        [Fact]
        public void PackageMapExportTest()
        {
            var packet = @"Packets/PackageMapExport/packet2-layoutexport.dump";
            using (var stream = File.Open(packet, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var replay = new Replay()
                {
                    Header = new Header()
                    {
                        EngineNetworkVersion = EngineNetworkVersionHistory.HISTORY_UPDATE9,
                        NetworkVersion = NetworkVersionHistory.HISTORY_UPDATE9,
                        Flags = (ReplayHeaderFlags)3,
                    }
                };

                using (var reader = new FortniteBinaryDecompressor(stream, replay))
                {
                    using (var ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        var playbackPacket = new PlaybackPacket()
                        {
                            Data = ms.ToArray()
                        };
                        reader.ReceivedRawPacket(playbackPacket);
                    }
                }
            }
        }
    }
}
