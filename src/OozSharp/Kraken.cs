/*
=== Kraken Decompressor for Windows ===
Converted to C# for Fortnite by SL-x-TnT, original source code available at https://github.com/powzix/ooz.

Copyright (C) 2016, Powzix
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using OozSharp.Exceptions;
using OozSharp.Extensions;
using System;

namespace OozSharp;

/// <summary>
/// Oodle Kraken decompression
/// </summary>
public unsafe class Kraken
{
    /// <summary>
    /// Decompression for the <paramref name="compressedInput"/>.
    /// </summary>
    /// <param name="compressedInput"></param>
    /// <param name="uncompressedSize"></param>
    /// <returns>Decompressed byte[]</returns>
    public ReadOnlyMemory<byte> Decompress(ReadOnlySpan<byte> compressedInput, int uncompressedSize)
    {
        using var decoder = new KrakenDecoder();

        var decompressedBuffer = new byte[uncompressedSize];
        var remainingBytes = uncompressedSize;
        var sourceLength = compressedInput.Length;
        var destinationOffset = 0;

        fixed (byte* decompressedBufferPtr = decompressedBuffer)
        fixed (byte* compressedInputPtr = compressedInput)
        fixed (byte* scratchPtr = decoder.Scratch)
        {
            var sourceStart = compressedInputPtr;
            var decompressBufferStart = decompressedBufferPtr;

            while (remainingBytes != 0)
            {
                if (!DecodeStep(decoder, decompressedBufferPtr, destinationOffset, remainingBytes, sourceStart, sourceLength, scratchPtr))
                {
                    throw new DecoderException($"Failed DecodeStep method");
                }

                sourceStart += decoder.SourceUsed;
                sourceLength -= decoder.SourceUsed;

                destinationOffset += decoder.DestinationUsed;
                remainingBytes -= decoder.DestinationUsed;
            }
        }

        return decompressedBuffer;
    }

    private bool DecodeStep(KrakenDecoder decoder, byte* destination, int destinationOffset, int remainingDestinationBytes, byte* source, int sourceBytesleft, byte* scratch)
    {
        var sourceIn = source;
        var sourceEnd = source + sourceBytesleft;
        if ((destinationOffset & 0x3FFFF) == 0)
        {
            decoder.Header = new KrakenHeader(source);

            source += 2;
        }

        // Only need Mermaid for Fortnite
        //"Oodle initializing compressor with Mermaid, level Normal, SpaceSpeed tradeoff 256"
        var isKrakenDecoder = decoder.Header.DecoderType == DecoderTypes.Mermaid;
        var destinationBytesLeft = Math.Min(isKrakenDecoder ? 0x40000 : 0x4000, remainingDestinationBytes);

        if (decoder.Header.Uncompressed)
        {
            if (sourceEnd - source < destinationBytesLeft)
            {
                throw new DecoderException($"DecodeStep: sourceEnd - source ({sourceEnd - source}) < destinationBytesLeft ({destinationBytesLeft})");
            }

            //throw new NotImplementedException($"memmove(dst_start + offset, src, dst_bytes_left);");
            Buffer.MemoryCopy(source, destination + destinationOffset, sourceBytesleft, sourceBytesleft);

            decoder.SourceUsed = (int) ((source - sourceIn) + destinationBytesLeft);
            decoder.DestinationUsed = destinationBytesLeft;

            return true;

        }

        KrakenQuantumHeader quantumHeader;

        if (isKrakenDecoder)
        {
            quantumHeader = new KrakenQuantumHeader(source, decoder.Header.UseChecksums, out var bytesRead);
            source += bytesRead;
        }
        else
        {
            throw new DecoderException($"Decoder type {decoder.Header.DecoderType} not supported");
        }

        if (source > sourceEnd)
        {
            throw new DecoderException($"Index out of range of source array");
        }

        // Too few bytes in buffer to make any progress?
        if (sourceEnd - source < quantumHeader.CompressedSize)
        {
            decoder.SourceUsed = 0;
            decoder.DestinationUsed = 0;

            return true;
        }

        if (quantumHeader.CompressedSize > remainingDestinationBytes)
        {
            throw new DecoderException($"Invalid compression size CompressedSize > RemainingDestinationLength. {quantumHeader.CompressedSize} > {remainingDestinationBytes}");
        }

        if (quantumHeader.CompressedSize == 0)
        {
            if (quantumHeader.WholeMatchDistance != 0)
            {
                if (quantumHeader.WholeMatchDistance > destinationOffset)
                {
                    throw new DecoderException($"WholeMatchDistance > destinationOffset. {quantumHeader.WholeMatchDistance} > {destinationOffset}");
                }

                throw new NotImplementedException($"Kraken_CopyWholeMatch(dst_start + offset, qhdr.whole_match_distance, dst_bytes_left);");
            }
            else
            {
                var val = quantumHeader.Checksum;
                Buffer.MemoryCopy(&val, destination + destinationOffset, destinationBytesLeft, destinationBytesLeft);
            }

            decoder.SourceUsed = (int) (source - sourceIn);
            decoder.DestinationUsed = destinationBytesLeft;

            return true;
        }

        if (decoder.Header.UseChecksums)
        {
            var checksum = GetCrc(source, quantumHeader.CompressedSize) & 0xFFFFFF;

            if (checksum != quantumHeader.Checksum)
            {
                throw new DecoderException($"Invalid checksum. Found {checksum} need {quantumHeader.Checksum}");
            }
        }

        if (quantumHeader.CompressedSize == destinationBytesLeft)
        {
            decoder.SourceUsed = (int) ((source - sourceIn) + destinationBytesLeft);
            decoder.DestinationUsed = destinationBytesLeft;

            throw new NotImplementedException($"memmove(dst_start + offset, src, dst_bytes_left);");
        }

        int numBytes;

        switch (decoder.Header.DecoderType)
        {
            case DecoderTypes.Mermaid:
                numBytes = MermaidDecodeQuantum(destination + destinationOffset, destination + destinationOffset + destinationBytesLeft, destination,
                    source, source + quantumHeader.CompressedSize, scratch, decoder.ScratchSize);
                break;
            default:
                throw new DecoderException($"Decoder type {decoder.Header.DecoderType} currently not supported");
        }


        if (numBytes != quantumHeader.CompressedSize)
        {
            throw new DecoderException($"Invalid number of bytes decompressed. {numBytes} != {quantumHeader.CompressedSize}");
        }


        decoder.SourceUsed = (int) (source - sourceIn) + numBytes;
        decoder.DestinationUsed = destinationBytesLeft;

        return true;
    }

    private uint GetCrc(byte* source, uint compressedSize) => throw new NotImplementedException();

    private int DecodeBytes(byte** output, byte* source, byte* sourceEnd, int* decodedSize, uint outputSize, bool forceMemmove, byte* scratch, byte* scratchEnd)
    {
        var sourceOrg = source;
        int sourceSize;

        if (sourceEnd - source < 2)
        {
            throw new DecoderException($"DecodeBytes: Too few bytes ({sourceEnd - source}) remaining");
        }

        var chunkType = (source[0] >> 4) & 0x7;

        if (chunkType == 0)
        {
            if (source[0] >= 0x80)
            {
                // In this mode, memcopy stores the length in the bottom 12 bits.
                sourceSize = ((source[0] << 8) | source[1]) & 0xFFF;
                source += 2;
            }
            else
            {
                if (sourceEnd - source < 3)
                {
                    throw new DecoderException($"DecodeBytes: Too few bytes ({sourceEnd - source}) remaining");
                }

                sourceSize = ((source[0] << 16 | source[1] << 8) | source[2]);

                if ((sourceSize & ~0x3ffff) > 0)
                {
                    throw new DecoderException($"Reserved bits must not be set");
                }

                source += 3;
            }

            if (sourceSize > outputSize || sourceEnd - source < sourceSize)
            {
                throw new DecoderException($"sourceSize ({sourceSize}) > outputSize ({outputSize}) || sourceEnd - source ({sourceEnd - source}) < sourceSize ({sourceSize})");
            }

            *decodedSize = sourceSize;

            if (forceMemmove)
            {
                throw new NotImplementedException($"Memmove not implemented");
            }
            else
            {
                *output = source;

                return (int) (source + sourceSize - sourceOrg);
            }
        }

        throw new NotImplementedException("DecodeBytes");
    }

    private int MermaidDecodeQuantum(byte* destination, byte* destinationEnd, byte* destinationStart, byte* source, byte* sourceEnd, byte* temp, int tempSize)
    {
        var tempEnd = temp + tempSize;
        var sourceIn = source;
        var mode = 0;
        var chunkHeader = 0;
        var destinationCount = 0;
        var sourceUsed = 0;
        var writtenBytes = 0;

        while (destinationEnd - destination != 0)
        {
            destinationCount = (int) (destinationEnd - destination);

            destinationCount = destinationCount > 0x20000 ? 0x20000 : destinationCount;

            if (sourceEnd - source < 4)
            {
                throw new DecoderException($"Less than 4 bytes remaining in source. Remaining: {sourceEnd - source}");
            }

            chunkHeader = source[2] | source[1] << 8 | source[0] << 16;

            if (!((chunkHeader & 0x800000) > 0))
            {
                //Stored without any match copying.
                var outDestination = destination;

                throw new NotImplementedException("src_used = Kraken_DecodeBytes(&out, src, src_end, &written_bytes, dst_count, false, temp, temp_end);");

                if (sourceUsed < 0 || writtenBytes != destinationCount)
                {
                    return -1;
                }
            }
            else
            {
                source += 3;
                sourceUsed = chunkHeader & 0x7FFFF;
                mode = (chunkHeader >> 19) & 0xF;

                if (sourceEnd - source < sourceUsed)
                {
                    throw new DecoderException($"Not enough source bytes remaining. Have {sourceEnd - source}. Need {sourceUsed}");
                }

                if (sourceUsed < destinationCount)
                {
                    var tempUsage = 2 * destinationCount + 32;

                    tempUsage = tempUsage > 0x40000 ? 0x40000 : tempUsage;

                    //Mermaid_ReadLzTable
                    if (!MermaidReadLzTable(mode,
                        source, source + sourceUsed,
                        destination, destinationCount, destination - destinationStart,
                        temp + sizeof(MermaidLzTable), temp + tempUsage, (MermaidLzTable*) temp))
                    {
                        throw new DecoderException($"Failed to run MermaidReadLzTable");
                    }

                    //Mermaid_ProcessLzRuns
                    if (!MermaidProcessLzRuns(mode,
                        source,
                        source + sourceUsed,
                        destination, destinationCount,
                        destination - destinationStart, destinationEnd,
                        (MermaidLzTable*) temp))
                    {
                        throw new DecoderException($"Failed to run MermaidProcessLzRuns");
                    }
                }
                else if (sourceUsed > destinationCount || mode != 0)
                {
                    throw new DecoderException($"Used bytes ({sourceUsed}) > destinationCount ({destinationCount} or Mode ({mode}) != 0");
                }
                else
                {
                    Buffer.MemoryCopy(source, destination, destinationCount, destinationCount);
                }
            }

            source += sourceUsed;
            destination += destinationCount;

        }

        return (int) (source - sourceIn);
    }

    private bool MermaidReadLzTable(int mode, byte* source, byte* sourceEnd, byte* destination, int destinationSize, long offset, byte* scratch, byte* scratchEnd, MermaidLzTable* lz)
    {
        byte* scratchOut;
        int decodeCount;
        int numBytes;
        uint temp;
        uint offset32Size2;
        uint offset32Size1;

        if (mode > 1)
        {
            return false;
        }

        if (sourceEnd - source < 10)
        {
            return false;
        }

        if (offset == 0)
        {
            Util.Copy64(destination, source);

            destination += 8;
            source += 8;
        }

        //Decode lit stream
        scratchOut = scratch;

        numBytes = DecodeBytes(&scratchOut, source, sourceEnd, &decodeCount, (uint) Math.Min(scratchEnd - scratch, destinationSize), false, scratch, scratchEnd);

        source += numBytes;
        lz->LitStream = scratchOut;
        lz->LitStreamEnd = scratchOut + decodeCount;
        scratch += decodeCount;

        //Decode flag stream
        scratchOut = scratch;
        numBytes = DecodeBytes(&scratchOut, source, sourceEnd, &decodeCount, (uint) Math.Min(scratchEnd - scratch, destinationSize), false, scratch, scratchEnd);

        source += numBytes;
        lz->CmdStream = scratchOut;
        lz->CmdStreamEnd = scratchOut + decodeCount;
        scratch += decodeCount;

        lz->CmdStream2OffsetsEnd = (uint) decodeCount;

        if (destinationSize <= 0x10000)
        {
            lz->CmdStream2Offsets = (uint) decodeCount;
        }
        else
        {
            if (sourceEnd - source < 2)
            {
                throw new DecoderException($"MermaidReadLzTable: Too few bytes ({sourceEnd - source}) remaining");
            }

            lz->CmdStream2Offsets = *(ushort*) source;

            source += 2;

            if (lz->CmdStream2Offsets > lz->CmdStream2OffsetsEnd)
            {
                throw new DecoderException($"MermaidReadLzTable: lz->CmdStream2Offsets ({lz->CmdStream2Offsets}) > lz->CmdStream2OffsetsEnd ({lz->CmdStream2OffsetsEnd})");
            }
        }

        if (sourceEnd - source < 2)
        {
            throw new DecoderException($"MermaidReadLzTable: Too few bytes ({sourceEnd - source}) remaining");
        }

        int off16Count = *(ushort*) source;

        if (off16Count == 0xFFFF)
        {
            byte* offset16Low;
            byte* offset16High;
            int offset16LowCount;
            int offset16HighCount;

            source += 2;
            offset16High = scratch;
            numBytes = DecodeBytes(&offset16High, source, sourceEnd, &offset16HighCount, (uint) Math.Min(scratchEnd - scratch, destinationSize >> 1), false, scratch, scratchEnd);

            source += numBytes;
            scratch += offset16HighCount;

            offset16Low = scratch;
            numBytes = DecodeBytes(&offset16Low, source, sourceEnd, &offset16LowCount, (uint) Math.Min(scratchEnd - scratch, destinationSize >> 1), false, scratch, scratchEnd);

            source += numBytes;
            scratch += offset16LowCount;

            if (offset16LowCount != offset16HighCount)
            {
                throw new DecoderException($"MermaidReadLzTable: offset16LowCount ({offset16LowCount}) != offset16HighCount ({offset16HighCount})");
            }

            scratch = Util.AlignPointer(scratch, 2);
            lz->Offset16Stream = (ushort*) scratch;

            if (scratch + offset16LowCount * 2 > scratchEnd)
            {
                throw new DecoderException($"MermaidReadLzTable: scratch + offset16LowCount * 2 > scratchEnd");
            }

            scratch += offset16LowCount * 2;
            lz->Offset16StreamEnd = (ushort*) scratch;

            MermaidCombineOffset16(lz->Offset16Stream, offset16LowCount, offset16Low, offset16High);
        }
        else
        {
            lz->Offset16Stream = (ushort*) (source + 2);
            source += 2 + off16Count * 2;
            lz->Offset16StreamEnd = (ushort*) source;
        }

        if (sourceEnd - source < 3)
        {
            throw new DecoderException($"MermaidReadLzTable: Too few bytes ({sourceEnd - source}) remaining");
        }

        temp = (uint) (source[0] | source[1] << 8 | source[2] << 16);

        source += 3;

        if (temp != 0)
        {
            offset32Size1 = temp >> 12;
            offset32Size2 = temp & 0xFFF;

            if (offset32Size1 == 4095)
            {
                if (sourceEnd - source < 2)
                {
                    throw new DecoderException($"MermaidReadLzTable: Too few bytes ({sourceEnd - source}) remaining");
                }

                offset32Size1 = *(ushort*) source;
                source += 2;
            }

            if (offset32Size2 == 4095)
            {
                if (sourceEnd - source < 2)
                {
                    throw new DecoderException($"MermaidReadLzTable: Too few bytes ({sourceEnd - source}) remaining");
                }

                offset32Size2 = *(ushort*) source;
                source += 2;
            }

            lz->Offset32Stream1Size = offset32Size1;
            lz->Offset32Stream2Size = offset32Size2;

            if (scratch + 4 * (offset32Size1 + offset32Size2) + 64 > scratchEnd)
            {
                throw new DecoderException($"MermaidReadLzTable: Not enough remaining scratch space");
            }

            scratch = Util.AlignPointer(scratch, 4);

            lz->Offset32Stream1 = (uint*) scratch;
            scratch += offset32Size1 * 4;

            // store dummy bytes after for prefetcher.
            ((ulong*) scratch)[0] = 0;
            ((ulong*) scratch)[1] = 0;
            ((ulong*) scratch)[2] = 0;
            ((ulong*) scratch)[3] = 0;
            scratch += 32;

            lz->Offset32Stream2 = (uint*) scratch;
            scratch += offset32Size2 * 4;

            // store dummy bytes after for prefetcher.
            ((ulong*) scratch)[0] = 0;
            ((ulong*) scratch)[1] = 0;
            ((ulong*) scratch)[2] = 0;
            ((ulong*) scratch)[3] = 0;
            scratch += 32;

            numBytes = MermaidDecodeFarOffsets(source, sourceEnd, lz->Offset32Stream1, lz->Offset32Stream1Size, offset);

            source += numBytes;

            numBytes = MermaidDecodeFarOffsets(source, sourceEnd, lz->Offset32Stream2, lz->Offset32Stream2Size, offset + 0x10000);

            source += numBytes;
        }
        else
        {
            if (scratchEnd - scratch < 32)
            {
                throw new DecoderException($"MermaidReadLzTable: Too few bytes ({sourceEnd - source}) remaining");
            }


            lz->Offset32Stream1Size = 0;
            lz->Offset32Stream2Size = 0;
            lz->Offset32Stream1 = (uint*) scratch;
            lz->Offset32Stream2 = (uint*) scratch;

            // store dummy bytes after for prefetcher.
            ((ulong*) scratch)[0] = 0;
            ((ulong*) scratch)[1] = 0;
            ((ulong*) scratch)[2] = 0;
            ((ulong*) scratch)[3] = 0;
        }

        lz->LengthStream = source;

        return true;
    }

    private bool MermaidProcessLzRuns(int mode, byte* source, byte* sourceEnd, byte* destination, int destinationSize, long offset, byte* destinationEnd, MermaidLzTable* lz)
    {
        var iteration = 0;
        var destinationStart = destination - offset;
        var savedDist = -8;
        byte* sourceCurrent = null;

        for (iteration = 0; iteration != 2; iteration++)
        {
            var destinationSizeCurrent = destinationSize;

            destinationSizeCurrent = destinationSizeCurrent > 0x10000 ? 0x10000 : destinationSizeCurrent;

            if (iteration == 0)
            {
                lz->Offset32Stream = lz->Offset32Stream1;
                lz->Offset32StreamEnd = lz->Offset32Stream1 + lz->Offset32Stream1Size * 4;
                lz->CmdStreamEnd = lz->CmdStream + lz->CmdStream2Offsets;
            }
            else
            {
                lz->Offset32Stream = lz->Offset32Stream2;
                lz->Offset32StreamEnd = lz->Offset32Stream2 + lz->Offset32Stream2Size * 4;
                lz->CmdStreamEnd = lz->CmdStream + lz->CmdStream2OffsetsEnd;

                lz->CmdStream += lz->CmdStream2Offsets;
            }

            if (mode == 0)
            {
                throw new NotImplementedException($"MermaidProcessLzRuns: Mode 0 not implemented currently");
            }
            else
            {
                sourceCurrent = MermaidMode1(destination, destinationSizeCurrent, destinationEnd, destinationStart, sourceEnd, lz, &savedDist, (offset == 0) && (iteration == 0) ? 8 : 0);
            }

            destination += destinationSizeCurrent;
            destinationSize -= destinationSizeCurrent;

            if (destinationSize == 0)
            {
                break;
            }
        }

        if (sourceCurrent != sourceEnd)
        {
            throw new DecoderException($"MermaidProcessLzRuns: Failed to read decompress source bytes");
        }

        return true;
    }

    private byte* MermaidMode1(byte* destination, int destinationSize, byte* destinationEndPtr, byte* destinationStart, byte* sourceEnd, MermaidLzTable* lz, int* savedDist, int startOff)
    {
        var destinationEnd = destination + destinationSize;
        var cmdStream = lz->CmdStream;
        var cmdStreamEnd = lz->CmdStreamEnd;
        var lengthStream = lz->LengthStream;
        var litStream = lz->LitStream;
        var litStreamEnd = lz->LitStreamEnd;
        var off16Stream = lz->Offset16Stream;
        var off16StreamEnd = lz->Offset16StreamEnd;
        var off32Stream = lz->Offset32Stream;
        var off32StreamEnd = lz->Offset32StreamEnd;
        var recentOffs = *savedDist;
        byte* match;
        int length;
        var destinationBegin = destination;

        destination += startOff;

        var test = cmdStreamEnd - cmdStream;

        while (cmdStream < cmdStreamEnd)
        {
            uint flag = *cmdStream++;

            if (flag >= 24)
            {
                uint newDist = *off16Stream;
                var useDistance = (flag >> 7) - 1;
                var litLen = flag & 7;

                Util.Copy64(destination, litStream);

                destination += litLen;
                litStream += litLen;

                recentOffs ^= (int) (useDistance & (uint) (recentOffs ^ -newDist));

                off16Stream = (ushort*) ((byte*) off16Stream + (useDistance & 2));
                match = destination + recentOffs;

                Util.Copy64(destination, match);
                Util.Copy64(destination + 8, match + 8);

                destination += (flag >> 3) & 0xF;

            }
            else if (flag > 2)
            {
                length = (int) (flag + 5);

                if (off32Stream == off32StreamEnd)
                {
                    throw new DecoderException($"MermaidMode1: off32Stream == off32StreamEnd");
                }

                match = destinationBegin - *off32Stream++;
                recentOffs = (int) (match - destination);

                if (destinationEnd - destination < length)
                {
                    throw new DecoderException($"MermaidMode1: destinationEnd - destination < length");
                }

                Util.Copy64(destination, match);
                Util.Copy64(destination + 8, match + 8);
                Util.Copy64(destination + 16, match + 16);
                Util.Copy64(destination + 24, match + 24);

                destination += length;
            }
            else if (flag == 0)
            {
                if (sourceEnd - lengthStream == 0)
                {
                    throw new DecoderException($"MermaidMode1: Not enough source bytes remaining. Have {sourceEnd - lengthStream}");
                }

                length = *lengthStream;

                if (length > 251)
                {
                    if (sourceEnd - lengthStream < 3)
                    {
                        throw new DecoderException($"MermaidMode1: Not enough source bytes remaining. Have {sourceEnd - lengthStream}. Need 3");
                    }

                    length += *(ushort*) (lengthStream + 1) * 4;
                    lengthStream += 2;
                }

                lengthStream += 1;

                length += 64;

                if (destinationEnd - destination < length || litStreamEnd - litStream < length)
                {
                    throw new DecoderException($"MermaidMode1: destinationEnd - destination < length || litStreamEnd - litStream < length");
                }

                do
                {
                    Util.Copy64(destination, litStream);
                    Util.Copy64(destination + 8, litStream + 8);

                    destination += 16;
                    litStream += 16;
                    length -= 16;
                } while (length > 0);

                destination += length;
                litStream += length;
            }
            else if (flag == 1)
            {
                if (sourceEnd - lengthStream == 0)
                {
                    throw new DecoderException($"MermaidMode1: Not enough source bytes remaining. Have {sourceEnd - lengthStream}");
                }

                length = *lengthStream;

                if (length > 251)
                {
                    if (sourceEnd - lengthStream < 3)
                    {
                        throw new DecoderException($"MermaidMode1: Not enough source bytes remaining. Have {sourceEnd - lengthStream}. Need 3");
                    }

                    length += *(ushort*) (lengthStream + 1) * 4;
                    lengthStream += 2;
                }

                lengthStream += 1;
                length += 91;

                if (off16Stream == off16StreamEnd)
                {
                    throw new DecoderException($"MermaidMode1: off16Stream == off16StreamEnd");
                }

                match = destination - *off16Stream++;
                recentOffs = (int) (match - destination);

                do
                {
                    Util.Copy64(destination, match);
                    Util.Copy64(destination + 8, match + 8);

                    destination += 16;
                    match += 16;
                    length -= 16;
                } while (length > 0);

                destination += length;
            }
            else
            {
                if (sourceEnd - lengthStream == 0)
                {
                    throw new DecoderException($"MermaidMode1: sourceEnd - lengthStream == 0");
                }

                length = *lengthStream;

                if (length > 251)
                {
                    if (sourceEnd - lengthStream < 3)
                    {
                        throw new DecoderException($"MermaidMode1: Not enough source bytes remaining. Have {sourceEnd - lengthStream}. Need 3");
                    }

                    length += *(ushort*) (lengthStream + 1) * 4;
                    lengthStream += 2;
                }

                lengthStream += 1;
                length += 29;

                if (off32Stream == off32StreamEnd)
                {
                    throw new DecoderException($"MermaidMode1: off32Stream == off32StreamEnd");
                }

                match = destinationBegin - *off32Stream++;
                recentOffs = (int) (match - destination);

                do
                {
                    Util.Copy64(destination, match);
                    Util.Copy64(destination + 8, match + 8);

                    destination += 16;
                    match += 16;
                    length -= 16;
                } while (length > 0);

                destination += length;
            }
        }

        length = (int) (destinationEnd - destination);

        if (length >= 8)
        {
            do
            {
                Util.Copy64(destination, litStream);

                destination += 8;
                litStream += 8;
                length -= 8;
            } while (length >= 8);
        }

        if (length > 0)
        {
            do
            {
                *destination++ = *litStream++;
            } while (--length > 0);
        }

        *savedDist = recentOffs;
        lz->LengthStream = lengthStream;
        lz->Offset16Stream = off16Stream;
        lz->LitStream = litStream;

        return lengthStream;
    }

    private void MermaidCombineOffset16(ushort* destination, int size, byte* lo, byte* hi)
    {
        for (var i = 0; i != size; i++)
        {
            destination[i] = (ushort) (lo[i] + hi[i] * 256);
        }
    }

    private int MermaidDecodeFarOffsets(byte* source, byte* sourceEnd, uint* output, uint outputSize, long offset)
    {
        var sourceCurrent = source;
        uint i;
        uint off;

        if (offset < (0xC00000 - 1))
        {
            for (i = 0; i != outputSize; i++)
            {
                if (sourceEnd - sourceCurrent < 3)
                {
                    throw new DecoderException($"MermaidDecodeFarOffsets: Too few bytes ({sourceEnd - source}) remaining");
                }

                off = (uint) (sourceCurrent[0] | sourceCurrent[1] << 8 | sourceCurrent[2] << 16);
                sourceCurrent += 3;

                output[i] = off;

                if (off > offset)
                {
                    throw new DecoderException($"MermaidDecodeFarOffsets: off ({off}) > offset ({offset})");
                }
            }

            return (int) (sourceCurrent - source);
        }

        for (i = 0; i != outputSize; i++)
        {
            if (sourceEnd - sourceCurrent < 3)
            {
                throw new DecoderException($"MermaidDecodeFarOffsets: Too few bytes ({sourceEnd - source}) remaining");
            }

            off = (uint) (sourceCurrent[0] | sourceCurrent[1] << 8 | sourceCurrent[2] << 16);
            sourceCurrent += 3;

            if (off >= 0xc00000)
            {
                if (sourceCurrent == sourceEnd)
                {
                    throw new DecoderException($"MermaidDecodeFarOffsets: No remaining bytes");
                }

                off += (uint) (*sourceCurrent++ << 22);
            }

            output[i] = off;

            if (off > offset)
            {
                throw new DecoderException($"MermaidDecodeFarOffsets: off ({off}) > offset ({offset})");
            }
        }

        return (int) (sourceCurrent - source);
    }
}
