using BenchmarkDotNet.Attributes;
using System;
using System.IO;

namespace BenchmarkReader;

[MemoryDiagnoser]
public class BenchmarkBinaryReader
{
    private readonly Unreal.Core.BinaryReader Reader;

    public BenchmarkBinaryReader()
    {
        Random rnd = new();
        var b = new byte[1000];
        rnd.NextBytes(b);
        var ms = new MemoryStream(b);
        Reader = new Unreal.Core.BinaryReader(ms);
    }

    [Benchmark]
    public void ReadByte()
    {
        Reader.Seek(0);
        Reader.ReadByte();
    }

    [Benchmark]
    public void ReadBytes()
    {
        Reader.Seek(0);
        Reader.ReadBytes(5);
    }

    [Benchmark]
    public void BinaryReader()
    {
        Random rnd = new();
        var b = new byte[1000];
        rnd.NextBytes(b);
        using var ms = new MemoryStream(b);
        var reader = new Unreal.Core.BinaryReader(ms);
    }
}
