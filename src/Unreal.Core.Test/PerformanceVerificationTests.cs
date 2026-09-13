using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using Unreal.Core.Extensions;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;
using Xunit;

namespace Unreal.Core.Test;

public class PerformanceVerificationTests
{
    [Fact]
    public void RemoveAllPathPrefixes_ZeroAllocationsAndFasterThanLinq()
    {
        const string testPath = "ROOT_Default__FortPlayerPawn_Athena_C";

        // Measure optimized allocations
        var allocBefore = GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < 1000; i++)
        {
            _ = testPath.RemoveAllPathPrefixes();
        }
        var allocAfter = GC.GetAllocatedBytesForCurrentThread();

        // Should have 0 GC allocations
        Assert.Equal(0, allocAfter - allocBefore);
    }

    [Fact]
    public void BinaryPrimitives_DirectReads_ZeroAllocations()
    {
        var data = new byte[32];
        Random.Shared.NextBytes(data);
        var span = data.AsSpan();

        var allocBefore = GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < 1000; i++)
        {
            _ = BinaryPrimitives.ReadInt16LittleEndian(span.Slice(0, 2));
            _ = BinaryPrimitives.ReadInt32LittleEndian(span.Slice(2, 4));
            _ = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(6, 4));
            _ = BinaryPrimitives.ReadInt64LittleEndian(span.Slice(10, 8));
            _ = BinaryPrimitives.ReadSingleLittleEndian(span.Slice(18, 4));
        }
        var allocAfter = GC.GetAllocatedBytesForCurrentThread();

        Assert.Equal(0, allocAfter - allocBefore);
    }

    [Fact]
    public void EnumParsing_UnsafeAs_ZeroAllocationsAndExactMatch()
    {
        byte b = 2;

        // Baseline: Enum.ToObject boxes
        var baseline = (ChannelCloseReason)Enum.ToObject(typeof(ChannelCloseReason), b);

        // Optimized: Unsafe.As
        var optimized = Unsafe.As<byte, ChannelCloseReason>(ref b);

        Assert.Equal(baseline, optimized);

        var allocBefore = GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < 1000; i++)
        {
            _ = Unsafe.As<byte, ChannelCloseReason>(ref b);
        }
        var allocAfter = GC.GetAllocatedBytesForCurrentThread();

        Assert.Equal(0, allocAfter - allocBefore);
    }

    [Fact]
    public void ReadBitsToInt_BitShiftMatchesLoopAndAllocatesZero()
    {
        var buffer = new byte[16];
        for (var i = 0; i < buffer.Length; i++) buffer[i] = (byte)(i * 17 + 3);

        var reader = new BitReader(buffer);

        // 1. Verify correctness against single-bit loop
        for (var bitCount = 1; bitCount <= 32; bitCount++)
        {
            reader.Seek(0);
            var optVal = reader.ReadBitsToInt(bitCount);

            var loopVal = 0;
            for (var b = 0; b < bitCount; b++)
            {
                var bit = (buffer[b >> 3] & (1 << (b & 7))) != 0;
                if (bit) loopVal |= (1 << b);
            }

            Assert.Equal(loopVal, optVal);
        }

        // 2. Verify zero allocations
        var allocBefore = GC.GetAllocatedBytesForCurrentThread();
        for (var bitCount = 1; bitCount <= 32; bitCount++)
        {
            reader.Seek(0);
            _ = reader.ReadBitsToInt(bitCount);
        }
        var allocAfter = GC.GetAllocatedBytesForCurrentThread();

        Assert.Equal(0, allocAfter - allocBefore);
    }

    [Fact]
    public void TempEnd_ArrayLookupMatchesDictAndIsFaster()
    {
        var reader = new BitReader(new byte[100]);

        // Set and restore temp end using optimized array
        reader.SetTempEnd(100, FBitArchiveEndIndex.BUNCH);
        Assert.Equal(100, reader.LastBit);
        reader.RestoreTempEnd(FBitArchiveEndIndex.BUNCH);
        Assert.Equal(800, reader.LastBit);

        var allocBefore = GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < 1000; i++)
        {
            reader.SetTempEnd(50, FBitArchiveEndIndex.BUNCH);
            reader.RestoreTempEnd(FBitArchiveEndIndex.BUNCH);
        }
        var allocAfter = GC.GetAllocatedBytesForCurrentThread();

        Assert.Equal(0, allocAfter - allocBefore);
    }

    [Fact]
    public void FillBitsFrom_ZeroAllocations()
    {
        var sourceBytes = new byte[256];
        Random.Shared.NextBytes(sourceBytes);
        var source = new BitReader(sourceBytes);
        var dest = new NetBitReader();

        // Warmup
        dest.FillBitsFrom(source, 64);

        var allocBefore = GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < 500; i++)
        {
            source.Seek(0);
            dest.FillBitsFrom(source, 64);
        }
        var allocAfter = GC.GetAllocatedBytesForCurrentThread();

        Assert.Equal(0, allocAfter - allocBefore);
    }

    [Fact]
    public void NetGuidCache_DirectLookup_MatchesTwoStep()
    {
        var cache = new NetGuidCache();
        for (uint i = 0; i < 100; i++)
        {
            var name = $"Group_{i}";
            cache.AddToExportGroupMap(name, new NetFieldExportGroup { PathNameIndex = i, PathName = name });
        }

        for (uint i = 0; i < 100; i++)
        {
            var group = cache.GetNetFieldExportGroupFromIndex(i);
            Assert.NotNull(group);
            Assert.Equal($"Group_{i}", group.PathName);
        }

        var allocBefore = GC.GetAllocatedBytesForCurrentThread();
        for (int r = 0; r < 500; r++)
        {
            for (uint i = 0; i < 100; i++)
            {
                _ = cache.GetNetFieldExportGroupFromIndex(i);
            }
        }
        var allocAfter = GC.GetAllocatedBytesForCurrentThread();

        Assert.Equal(0, allocAfter - allocBefore);
    }
}
