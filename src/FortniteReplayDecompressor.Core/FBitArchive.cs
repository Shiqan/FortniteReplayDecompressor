using System;
using System.IO;

namespace FortniteReplayReaderDecompressor.Core
{
    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/release/Engine/Source/Runtime/Core/Public/Serialization/Archive.h
    /// see https://github.com/EpicGames/UnrealEngine/blob/release/Engine/Source/Runtime/Core/Private/Serialization/Archive.cpp
    /// </summary>
    public class FBitArchive : FArchive
    {
        public FBitArchive(Stream input) : base(input)
        {

        }

        public override T ReadByteAsEnum<T>()
        {
            throw new NotImplementedException();
        }

        public override T ReadUInt32AsEnum<T>()
        {
            throw new NotImplementedException();
        }

        public override T[] ReadArray<T>(Func<T> func1)
        {
            throw new NotImplementedException();
        }

        public override ValueTuple<T, U>[] ReadTupleArray<T, U>(Func<T> func1, Func<U> func2)
        {
            throw new NotImplementedException();
        }

        public override string ReadFString()
        {
            throw new NotImplementedException();
        }

        public override string ReadBytesToString(int count)
        {
            throw new NotImplementedException();
        }

        public override string ReadGUID()
        {
            throw new NotImplementedException();
        }
        public override bool ReadInt32AsBoolean()
        {
            throw new NotImplementedException();
        }

        public override bool ReadUInt32AsBoolean()
        {
            throw new NotImplementedException();
        }

        public override uint ReadIntPacked()
        {
            throw new NotImplementedException();
        }

        public uint ReadVectorPacked()
        {
            throw new NotImplementedException();
        }
    }
}
