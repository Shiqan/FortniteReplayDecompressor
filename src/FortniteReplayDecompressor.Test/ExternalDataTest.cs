using FortniteReplayReaderDecompressor;
using FortniteReplayReaderDecompressor.Core;
using System.IO;
using Xunit;

namespace FortniteReplayDecompressor.Test
{
    public class ExternalDataTest
    {
        [Fact]
        public void ReadExternalDataTest()
        {
            Assert.True(false);
            //for (var i = 0; i < 5; i++)
            //{
            //    var data = $"ExternalData/externaldata{i}.dump";
            //    using (var stream = File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            //    {
            //        using (var ms = new MemoryStream())
            //        {
            //            stream.CopyTo(ms);
            //            var reader = new BitReader(ms.ToArray());
            //            reader.ReadExternalData();
            //            Assert.True(reader.AtEnd());
            //        }
            //    }
            //}
        }
    }
}
