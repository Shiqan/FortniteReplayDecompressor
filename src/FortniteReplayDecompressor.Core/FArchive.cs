using FortniteReplayReader.Core.Models.Enums;
using System;
using System.IO;

namespace FortniteReplayReaderDecompressor.Core
{
    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/release/Engine/Source/Runtime/Core/Public/Serialization/Archive.h
    /// see https://github.com/EpicGames/UnrealEngine/blob/release/Engine/Source/Runtime/Core/Private/Serialization/Archive.cpp
    /// </summary>
    public abstract class FArchive : BinaryReader
    {
        public EngineNetworkVersionHistory EngineNetworkVersion { get; set; }
        public ReplayHeaderFlags ReplayHeaderFlags { get; set; }
        public uint GameNetworkVersion { get; set; }
        public bool ArIsError { get; private set; }

        public FArchive(Stream input) : base(input)
        {

        }

        public virtual bool HasLevelStreamingFixes()
        {
            return ReplayHeaderFlags >= ReplayHeaderFlags.HasStreamingFixes;
        }

        public virtual bool AtEnd()
        {
            return BaseStream.Position >= BaseStream.Length;
        }

        public virtual bool IsError() { return ArIsError; }

        public abstract T ReadUInt32AsEnum<T>();
        public abstract T ReadByteAsEnum<T>();
        public abstract T[] ReadArray<T>(Func<T> func1);
        public abstract string ReadBytesToString(int count);
        public abstract string ReadFString();
        public abstract string ReadGUID();
        public abstract bool ReadInt32AsBoolean();
        public abstract uint ReadIntPacked();
        public abstract ValueTuple<T, U>[] ReadTupleArray<T, U>(Func<T> func1, Func<U> func2);
        public abstract bool ReadUInt32AsBoolean();

        public virtual void SkipBytes(uint byteCount)
        {
            BaseStream.Seek(byteCount, SeekOrigin.Current);
        }

        public virtual void SkipBytes(int byteCount)
        {
            BaseStream.Seek(byteCount, SeekOrigin.Current);
        }
    }
}
