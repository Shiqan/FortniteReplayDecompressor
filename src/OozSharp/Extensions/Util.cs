using System;
using System.Collections.Generic;
using System.Text;

namespace OozSharp.Extensions
{
    public unsafe static class Util
    {
        public static void Copy64(byte* destination, byte * source)
        {
            *(ulong*)destination = *(ulong*)source;
        }

        public static byte* AlignPointer(byte* p, int align)
        {
            return ((byte*)((*(IntPtr*)&p + (align - 1)).ToInt64() & ~(align - 1)));
        }
    }
}
