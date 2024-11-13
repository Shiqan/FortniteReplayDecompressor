using Unreal.Core.Models;
using Unreal.Core.Models.Enums;
using Xunit;

namespace Unreal.Core.Test;

public class NetBitReaderTest
{
    [Theory]
    [InlineData(new byte[] {
        0xD0, 0xD7, 0x07, 0x6F, 0xB0, 0xB3, 0x7F, 0x90, 0x01, 0xDD, 0x81, 0x0F,
        0xE2, 0x0E, 0x20 }, 118)]
    [InlineData(new byte[] {
        0x34, 0xC6, 0x7F, 0xF7, 0xA1, 0xB7, 0x8B, 0xB0, 0x9F, 0xA2, 0xFE, 0xDD,
        0xD9, 0x25 }, 110, VectorQuantization.RoundOneDecimal)]
    [InlineData(new byte[] {
        0x5A, 0x45, 0x13, 0xEF, 0x35, 0xFC, 0xA4, 0x4E, 0x77, 0xBF, 0x00, 0xDE,
        0x1D, 0xD6, 0xF2, 0x18, 0xB0, 0x95, 0x4C, 0xF5, 0xD1, 0xF0, 0x14, 0x7E,
        0xA7, 0x97, 0x1B, 0x01 }, 218,
        VectorQuantization.RoundWholeNumber, RotatorQuantization.ShortComponents, VectorQuantization.RoundTwoDecimals)]
    // meat vehicle season 11
    [InlineData(new byte[] {
        0x5B, 0xAE, 0xF0, 0x14, 0x44, 0x01, 0x1E, 0x47, 0x02, 0xBD, 0xA7, 0xFF,
        0x4B, 0x10, 0xBA, 0xFF, 0x03, 0x15, 0xA8 }, 152,
        VectorQuantization.RoundTwoDecimals, RotatorQuantization.ShortComponents)]
    // sniper rifle bullet season 11
    [InlineData(new byte[] {
        0x34, 0x88, 0xDF, 0x03, 0xE0, 0xE9, 0xCB, 0x3F, 0x92, 0x3B, 0x53, 0x3C,
        0x47, 0x61, 0xD6, 0x01 }, 122,
        VectorQuantization.RoundWholeNumber, RotatorQuantization.ByteComponents, VectorQuantization.RoundWholeNumber)]
    // supply drop season 11
    [InlineData(new byte[] {
        0x74, 0x20, 0x88, 0x53, 0x86, 0xDA, 0x16, 0xD8, 0x02, 0x40, 0x00, 0x38,
        0x2B, 0x00 }, 105,
        VectorQuantization.RoundWholeNumber, RotatorQuantization.ByteComponents, VectorQuantization.RoundWholeNumber)]
    // meat vehicle 11.40
    [InlineData(new byte[] {
        0xDA, 0x34, 0x06, 0xCA, 0x0A, 0xFE, 0x68, 0x40, 0x29, 0xBE, 0xB9, 0xFF,
        0x83, 0x55, 0x1A, 0xF9, 0x47, 0xF2, 0xBD, 0xBE, 0x54, 0x3B, 0xFB, 0x88,
        0xAB, 0xBF, 0x70, 0xB3, 0xCB, 0x02}, 236,
        VectorQuantization.RoundWholeNumber, RotatorQuantization.ShortComponents, VectorQuantization.RoundTwoDecimals)]
    [InlineData(new byte[] {
        0xA0, 0x65, 0x06, 0xE5, 0x68, 0x79, 0x0F, 0x60, 0xD8, 0x85, 0xFD, 0x05,
        0x15, 0x04 }, 111, VectorQuantization.RoundTwoDecimals, RotatorQuantization.ByteComponents, VectorQuantization.RoundWholeNumber, 
        EngineNetworkVersionHistory.CustomExports)]
    public void RepMovementTest(byte[] rawData, int bitCount,
        VectorQuantization locationQuantizationLevel = VectorQuantization.RoundTwoDecimals,
        RotatorQuantization rotationQuantizationLevel = RotatorQuantization.ByteComponents,
        VectorQuantization velocityQuantizationLevel = VectorQuantization.RoundWholeNumber,
        EngineNetworkVersionHistory engineNetworkVersion = EngineNetworkVersionHistory.HISTORY_INITIAL)
    {
        var reader = new NetBitReader(rawData, bitCount)
        {
            EngineNetworkVersion = engineNetworkVersion
        };

        reader.SerializeRepMovement(locationQuantizationLevel, rotationQuantizationLevel, velocityQuantizationLevel);
        Assert.False(reader.IsError);
        Assert.True(reader.AtEnd());
    }

    [Theory]
    [InlineData(new byte[] {
        0x08, 0x31, 0x00, 0x00, 0x00, 0x44, 0x45, 0x53, 0x4B, 0x54, 0x4F, 0x50,
        0x2D, 0x32, 0x32, 0x38, 0x4E, 0x47, 0x43, 0x35, 0x2D, 0x42, 0x39, 0x31,
        0x33, 0x37, 0x31, 0x30, 0x38, 0x34, 0x46, 0x46, 0x32, 0x46, 0x37, 0x45,
        0x35, 0x44, 0x36, 0x38, 0x38, 0x30, 0x31, 0x39, 0x35, 0x30, 0x35, 0x30,
        0x39, 0x41, 0x43, 0x31, 0x34, 0x00 }, 432, "DESKTOP-228NGC5-B91371084FF2F7E5D68801950509AC14")]
    [InlineData(new byte[] {
        0x11, 0x10, 0x37, 0xDF, 0x4A, 0x07, 0x98, 0xC2, 0x40, 0x2E, 0xAA, 0x62,
        0x69, 0x47, 0xEC, 0x29, 0x90, 0x3F}, 144, "37DF4A0798C2402EAA626947EC29903F")]
    [InlineData(new byte[] {
        0x29, 0x08, 0x25, 0x35, 0x43, 0x94, 0x31, 0x47, 0x40, 0x39}, 80, "2535439431474039")]
    public void NetUniqueIdTest(byte[] rawData, int bitCount, string expected)
    {
        var reader = new NetBitReader(rawData, bitCount);
        var result = reader.SerializePropertyNetId();
        Assert.Equal(expected, result);
        Assert.False(reader.IsError);
        Assert.True(reader.AtEnd());
    }


    [Theory]
    [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x44, 0x00, 0x00, 0x00, 0x44 }, 512, 512)]
    [InlineData(new byte[] { 0x80, 0x00, 0xA0, 0x48, 0x30, 0x1F, 0x9E, 0x48 }, 327684, 323833.5)]
    public void SerializePropertyVector2DTest(byte[] rawData, float x, float y)
    {
        var reader = new NetBitReader(rawData);
        var result = reader.SerializePropertyVector2D();
        var expected = new FVector2D(x, y);

        Assert.Equal(expected, result);
        Assert.False(reader.IsError);
        Assert.True(reader.AtEnd());
    }

    [Theory]
    [InlineData(new byte[] { 0x52, 0xEB }, 0.8384655, 1, 16)]
    [InlineData(new byte[] { 0x99, 0xA8 }, 0.31717888, 1, 16)]
    [InlineData(new byte[] { 0xB8, 0xB8 }, 0.44312876, 1, 16)]
    public void ReadFixedCompressedFloatTest(byte[] rawData, float expected, int maxValue, int numBits)
    {
        var reader = new NetBitReader(rawData);
        var result = reader.ReadFixedCompressedFloat(maxValue, numBits);
        Assert.Equal(expected, result);
        Assert.False(reader.IsError);
        Assert.True(reader.AtEnd());
    }
}
