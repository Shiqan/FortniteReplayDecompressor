using Unreal.Core.Models;
using Xunit;

namespace Unreal.Core.Test
{
    public class FGameplayCueParametersTest
    {
        [Fact]
        public void GameplayCueParametersTest()
        {
            byte[] rawData = {
                0x02, 0x70, 0xE1, 0x7A, 0x00, 0x10
            };

            var reader = new NetBitReader(rawData, 46);
            var hitResult = new FGameplayCueParameters();
            hitResult.Serialize(reader);

            Assert.True(reader.AtEnd());
            Assert.False(reader.IsError);
        }
    }
}
