using Unreal.Core.Models;

namespace Unreal.Core
{
    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/Serialization/BitArchive.h
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Private/Serialization/BitArchive.cpp
    /// </summary>
    public abstract class FBitArchive : FArchive
    {
        public abstract bool PeekBit();
        public abstract bool ReadBit();
        public abstract bool[] ReadBits(int bitCount);
        public abstract bool[] ReadBits(uint bitCount);
        public abstract uint ReadInt(int maxValue);
        public abstract FVector ReadPackedVector(int scaleFactor, int maxBits);
        public abstract FRotator ReadSerializeCompressed();
        public abstract void Mark();
        public abstract void Pop();
        public abstract int GetBitsLeft();
        public abstract void AppendDataFromChecked(bool[] data);
    }
}
