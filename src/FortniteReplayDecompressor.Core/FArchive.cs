using FortniteReplayReader.Core.Models.Enums;
using System;
using System.IO;

namespace FortniteReplayReaderDecompressor.Core
{
    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/release/Engine/Source/Runtime/Core/Public/Serialization/Archive.h
    /// see https://github.com/EpicGames/UnrealEngine/blob/release/Engine/Source/Runtime/Core/Private/Serialization/Archive.cpp
    /// </summary>
    public abstract class FArchive
    {
        public EngineNetworkVersionHistory EngineNetworkVersion { get; set; }
        public ReplayHeaderFlags ReplayHeaderFlags { get; set; }
        public NetworkVersionHistory NetworkVersion { get; set; }
        public bool ArIsError { get; protected set; }

        public virtual bool HasLevelStreamingFixes()
        {
            return ReplayHeaderFlags >= ReplayHeaderFlags.HasStreamingFixes;
        }

        public abstract bool AtEnd();
        public virtual bool IsError() { return ArIsError; }
        public abstract T ReadUInt32AsEnum<T>();
        public abstract T ReadByteAsEnum<T>();
        public abstract T[] ReadArray<T>(Func<T> func1);
        public abstract string ReadBytesToString(int count);
        public abstract ushort ReadUInt16();
        public abstract uint ReadUInt32();
        public abstract ulong ReadUInt64();
        public abstract short ReadInt16();
        public abstract int ReadInt32();
        public abstract long ReadInt64();
        public abstract float ReadSingle();
        public abstract string ReadFString();
        public abstract string ReadGUID();
        public abstract uint ReadIntPacked();
        public abstract ValueTuple<T, U>[] ReadTupleArray<T, U>(Func<T> func1, Func<U> func2);
        public abstract bool ReadBoolean();
        public abstract bool ReadInt32AsBoolean();
        public abstract bool ReadUInt32AsBoolean();
        public abstract byte ReadByte();
        public abstract sbyte ReadSByte();
        public abstract byte[] ReadBytes(int byteCount);
        public abstract byte[] ReadBytes(uint byteCount);
        public abstract void SkipBytes(uint byteCount);
        public abstract void SkipBytes(int byteCount);
        public abstract void Seek(int offset, SeekOrigin seekOrigin = SeekOrigin.Begin);
    }
}
