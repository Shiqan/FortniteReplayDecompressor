using FortniteReplayReaderDecompressor.Core.Models;
using System.IO;

namespace FortniteReplayReaderDecompressor
{
    public class ReplayReader
    {
        public Replay Read(string file, int offset)
        {
            using (var stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                return Read(stream, offset);
            }
        }

        public Replay Read(Stream stream, int offset)
        {
            var visitor = new FortniteReplayVisitor();
            using (var archive = new Core.BinaryReader(stream))
            {
                visitor.ReadReplay(archive);
            }

            return visitor.Replay;
        }

        public Replay Read(string file)
        {
            using (var stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                return Read(stream);
            }
        }

        public Replay Read(Stream stream)
        {
            var visitor = new FortniteReplayVisitor();
            using (var archive = new Core.BinaryReader(stream))
            {
                visitor.ReadReplay(archive);
            }

            return visitor.Replay;
        }
    }
}
