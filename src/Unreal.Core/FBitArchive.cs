using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace Unreal.Core
{
    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/Serialization/BitArchive.h
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Private/Serialization/BitArchive.cpp
    /// </summary>
    public abstract class FBitArchive : FArchive
    {
        public abstract bool PeekBit();
        public abstract byte PeekByte();
        public abstract bool ReadBit();
        public abstract bool[] ReadBits(int bitCount);
        public abstract bool[] ReadBits(uint bitCount);
        public abstract uint ReadSerializedInt(int maxValue);
        public abstract FVector ReadPackedVector(int scaleFactor, int maxBits);
        public abstract FRotator ReadRotation();
        public abstract FRotator ReadRotationShort();
        public abstract void SkipBits(int bitCount);
        public abstract void SkipBits(uint bitCount);
        public abstract void Mark();
        public abstract void Pop();
        public abstract int GetBitsLeft();
        public abstract void AppendDataFromChecked(bool[] data);
    }
}
