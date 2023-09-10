using FortniteReplayReader.Extensions;
using Xunit;

namespace FortniteReplayReader.Test;

public class UIntExtensionTest
{
    [Theory]
    [InlineData(77321u, "01:17")]
    [InlineData(115119u, "01:55")]
    [InlineData(522640u, "08:42")]
    public void MillisecondsToTimeStampTest(uint milliseconds, string expected) => Assert.Equal(expected, milliseconds.MillisecondsToTimeStamp());

    [Theory]
    [InlineData(371600u, 4)]
    [InlineData(5354u, 1)]
    public void CentimetersToDistanceTest(uint traveled, int distance) => Assert.Equal(distance, traveled.CentimetersToDistance());
}
