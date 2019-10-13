using Xunit;

namespace Unreal.Core.Test
{
    public class NetBitReaderTest
    {
        [Fact]
        public void RepMovementTest()
        {
            byte[] rawData = {
                0x50, 0x76, 0x07, 0x4F, 0xEB, 0xB0, 0x7F, 0x90, 0x01, 0xDD, 0x81, 0x0F,
                0xE2, 0x0E, 0x20
            };
            var reader = new NetBitReader(rawData, 79);
            reader.SerializeRepMovement();
            Assert.True(reader.AtEnd());
        }

        [Fact]
        public void RepMovementTest2()
        {
            byte[] rawData = {
                0xD0, 0xD7, 0x07, 0x6F, 0xB0, 0xB3, 0x7F, 0x90, 0x01, 0xDD, 0x81, 0x0F,
                0xE2, 0x0E, 0x20
            };

            var reader = new NetBitReader(rawData, 118);
            reader.SerializeRepMovement();
            Assert.True(reader.AtEnd());
        }
    }
}
