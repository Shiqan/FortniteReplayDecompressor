using System;

namespace OozSharp.Extensions;

public static unsafe class Util
{
    public static void Copy64(byte* destination, byte* source) => *(ulong*) destination = *(ulong*) source;

    public static byte* AlignPointer(byte* p, int align) => ((byte*) ((*(IntPtr*) &p + (align - 1)).ToInt64() & ~(align - 1)));
}
