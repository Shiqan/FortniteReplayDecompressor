using FortniteReplayReader;
using System.IO;

namespace FortniteReplayReaderDecompressor
{
    public class FortniteReplayDecompressor : FortniteBinaryReader
    {
        public FortniteReplayDecompressor(Stream input) : base(input)
        {
        }

        public FortniteReplayDecompressor(Stream input, int offset) : base(input)
        {

        }
        protected override void ParseCheckPoint()
        {
            var checkpointId = ReadFString();
            var checkpoint = ReadFString();
            using (var reader = Decompress())
            {
                reader.ReadUInt32();
            }
        }

        protected override void ParseReplayData()
        {
            var start = ReadUInt32();
            var end = ReadUInt32();
            var length = ReadUInt32();
            using (var reader = Decompress())
            {
                reader.ReadUInt32();
            }
        }

        private FortniteBinaryReader Decompress()
        {
            var decompressedSize = ReadInt32();
            var compressedSize = ReadInt32();
            var compressedBuffer = ReadBytes(compressedSize);
            var output = OodleBinding.DecompressReplayData(compressedBuffer, compressedBuffer.Length, decompressedSize);
            File.WriteAllBytes("replaydata.dump", output);
            return new FortniteBinaryReader(new MemoryStream(output));
        }
    }
}
