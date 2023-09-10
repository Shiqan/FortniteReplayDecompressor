using OozSharp;
using System;

namespace Unreal.Encryption;

/// <summary>  
/// see https://github.com/EpicGames/UnrealEngine/blob/release/Engine/Plugins/Runtime/PacketHandlers/CompressionComponents/Oodle/Source/OodleHandlerComponent/Private/OodleUtils.cpp 
/// </summary>
public static class Oodle
{
    private static readonly Kraken kraken = new();

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Plugins/Runtime/PacketHandlers/CompressionComponents/Oodle/Source/OodleHandlerComponent/Private/OodleUtils.cpp#L14
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="uncompressedSize"></param>
    /// <returns>byte[]</returns>
    public static ReadOnlyMemory<byte> DecompressReplayData(ReadOnlySpan<byte> buffer, int uncompressedSize) => kraken.Decompress(buffer, uncompressedSize);
}
