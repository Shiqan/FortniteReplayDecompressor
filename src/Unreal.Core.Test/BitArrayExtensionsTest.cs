using System.Collections;
using Unreal.Core.Extensions;
using Xunit;

namespace Unreal.Core.Test
{
    public class BitArrayExtensionsTest
    {
        [Fact]
        public void AppendBitArrayTest()
        {
            var original = new BitArray(new bool[4] { true, true, false, false });
            var toAppend = new BitArray(new bool[4] { true, true, false, false });
            var result = original.Append(toAppend);

            Assert.Equal(8, result.Length);
            Assert.True(result[1]);
            Assert.False(result[3]);
            Assert.True(result[4]);
            Assert.False(result[7]);
        }
        
        [Fact]
        public void AppendBoolArrayTest()
        {
            var original = new BitArray(new bool[4] { true, true, false, false });
            var toAppend = new bool[4] { true, true, false, false };
            var result = original.Append(toAppend);

            Assert.Equal(8, result.Length);
            Assert.True(result[1]);
            Assert.False(result[3]);
            Assert.True(result[4]);
            Assert.False(result[7]);
        }
    }
}
