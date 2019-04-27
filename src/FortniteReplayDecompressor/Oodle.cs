using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace FortniteReplayReader
{
    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/release/Engine/Plugins/Runtime/PacketHandlers/CompressionComponents/Oodle/Source/OodleHandlerComponent/Private/OodleUtils.cpp
    /// Thanks @Kuinox for finding the decompression settings
    /// </summary>
    public class Oodle
    {
        [DllImport("oo2core_5_win64")]
        private static extern int OodleLZ_Decompress(byte[] buffer, long bufferSize, byte[] outputBuffer, long outputBufferSize, uint a, uint b, ulong c, uint d, uint e, uint f, uint g, uint h, uint i, uint threadModule);

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Plugins/Runtime/PacketHandlers/CompressionComponents/Oodle/Source/OodleHandlerComponent/Private/OodleUtils.cpp#L14
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="size"></param>
        /// <param name="uncompressedSize"></param>
        /// <returns></returns>
        public static byte[] DecompressReplayData(byte[] buffer, int size, int uncompressedSize)
        {
            var decompressedBuffer = new byte[uncompressedSize];
            var decompressedCount = OodleLZ_Decompress(buffer, size, decompressedBuffer, uncompressedSize, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3);

            if (decompressedCount == uncompressedSize)
            {
                return decompressedBuffer;
            }
            else if (decompressedCount < uncompressedSize)
            {
                return decompressedBuffer.Take(decompressedCount).ToArray();
            }
            else
            {
                throw new Exception("There was an error while decompressing.");
            }
        }
    }
}
