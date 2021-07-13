#pragma warning disable SA1636 // File header copyright text should match
#pragma warning disable SA1512 // Single-line comments should not be followed by blank line
#pragma warning disable SA1515 // Single-line comment should be preceded by blank line
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable CA1822 // Mark members as static
// Uses code from unxwb by Luigi Auriemma for extracting songs from the XWB.
// Licensed under the GNU GPLv3 license. See unxwb.LICENSE file in the project root for full license information.
// Changes made: very poorly re-created to get just the entries needed from the XWB.

namespace PhosLibrary.Common
#pragma warning restore SA1515 // Single-line comment should be preceded by blank line
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;

    internal class XwbUtils
    {
        private static readonly int MaxEntries = 1110;
        private static readonly ushort CoeffsSize = 32;
        private static readonly uint[] Coeffs = { 0x00000100, 0xff000200, 0x00000000, 0x004000c0, 0x00000000f0, 0xff3001cc, 0xff180188 };

        private enum FormatMasks : uint
        {
            FormatTagMask = 0b0_00000000_000000000000000000_000_11,
            ChannelsMask = 0b0_00000000_000000000000000000_111_00,
            SamplesPerSecMask = 0b0_00000000_111111111111111111_000_00,
            BlockAlignMask = 0b0_11111111_000000000000000000_000_00,
            BitsPerSampleMask = 0b1_00000000_000000000000000000_000_00,
        }

        public static void ExtractSongs(string inputFile, string outputDir)
        {
            Directory.CreateDirectory(outputDir);

            WAVEBANKENTRY[] entries = WaveEntries(inputFile);
            string[] waveNames = WaveNames(inputFile);

            int[] uniqueEntries = new int[]
            {
                1022, // Everyday Sunshine
                1023, // Girl of the Hollow Forest
                1024, // A Sky Full of Stars
                1025, // Let's Hit the Beach
                318, // Junes Theme
                350, // The Poem for Everyone's Souls
                332, // Shiroku Pub Theme Instrumental
                323, // Shiroku Pub Theme Instrumental/Filtered
                287, // Someone Else's Man Filtered
                288, // Someone Else's Man Ambiance
                280, // Muscle Blues
                285, // It's Showtime

                1028, // Something quiz related
                1029, // Midnight Crossing Miracle Quiz

                316, // Indoor Rain SFX
                261, // Outdoor Rain SFX
                317, // Indoor Rain SFX [School]
                255, // Outdoor SFX [Summer]
                254, // Okina City SFX
            };

            int dataStartOffset = 0x7000;
            using BinaryReader reader = new(File.OpenRead(inputFile));
            reader.BaseStream.Seek(dataStartOffset, SeekOrigin.Begin);
            int filesCreated = 0;

            for (int currentIndex = 0; currentIndex < MaxEntries; currentIndex++)
            {
                WAVEBANKENTRY entry = entries[currentIndex];

                if (currentIndex > 60)
                {
                    if (!uniqueEntries.Contains(currentIndex))
                    {
                        continue;
                    }
                }

                reader.BaseStream.Seek(dataStartOffset + entry.PlayRegion.DwOffset, SeekOrigin.Begin);
                byte[] waveData = reader.ReadBytes((int)entry.PlayRegion.DwLength);
                uint wSubchunk1Size = (uint)(16 + CoeffsSize + 2);
                ushort wAudioFormat = (ushort)entry.Format.FormatTag;
                ushort wNumChannels = (ushort)entry.Format.Channels;
                uint wSampleRate = entry.Format.SamplesPerSec;
                ushort wBitsPerSample = 4;
                ushort wBlockAlign = (ushort)((entry.Format.BlockAlign + 22) * entry.Format.Channels);
                uint wSubchunk2Size = (uint)waveData.Length;
                ushort dw = (ushort)(((((uint)wBlockAlign - (7 * (uint)wNumChannels)) * 8) / ((uint)wBitsPerSample * (uint)wNumChannels)) + 2);
                uint wByteRate = (wSampleRate / dw) * wBlockAlign;

                WaveFile wav = new()
                {
                    Data = waveData,

                    Subchunk1Size = wSubchunk1Size,
                    AudioFormat = wAudioFormat,
                    NumChannels = wNumChannels,
                    SampleRate = wSampleRate,
                    BitsPerSample = wBitsPerSample,
                    BlockAlign = wBlockAlign,
                    Subchunk2Size = wSubchunk2Size,
                    ByteRate = wByteRate,

                    ChunkSize = 82 + wSubchunk2Size,
                };

                using (BinaryWriter writer = new(File.OpenWrite($@"{outputDir}\{waveNames[currentIndex]}.wav")))
                {
                    writer.Write(wav.ChunkID);
                    writer.Write(wav.ChunkSize);
                    writer.Write(wav.Format);
                    writer.Write(wav.Subchunk1ID);
                    writer.Write(wav.Subchunk1Size);
                    writer.Write(wav.AudioFormat);
                    writer.Write(wav.NumChannels);
                    writer.Write(wav.SampleRate);
                    writer.Write(wav.ByteRate);
                    writer.Write(wav.BlockAlign);
                    writer.Write(wav.BitsPerSample);

                    writer.Write((ushort)32);
                    writer.Write(dw);
                    writer.Write((ushort)Coeffs.Length);
                    foreach (var coeff in Coeffs)
                    {
                        writer.Write(coeff);
                    }

                    writer.Write(wav.FactChunk.ChunkID);
                    writer.Write(wav.FactChunk.ChunkSize);
                    writer.Write(wav.FactChunk.UncompressedSize);

                    writer.Write(wav.Subchunk2ID);
                    writer.Write(wav.Subchunk2Size);
                    writer.Write(wav.Data);
                }

                Output.Log(LogLevel.DEBUG, $"Created file: {waveNames[currentIndex]}.wav");

                filesCreated++;
            }

            Output.Log(LogLevel.INFO, $"Files created: {filesCreated}");
        }

        private static WAVEBANKENTRY[] WaveEntries(string inputFile)
        {
            Output.Log(LogLevel.INFO, "Parsing XWB wave entries");

            WAVEBANKENTRY[] entries = new WAVEBANKENTRY[MaxEntries];

            // BGM.xwb entries start offset
            int startOffset = 0x94;
            using (BinaryReader reader = new(File.OpenRead(inputFile)))
            {
                // seek to starting offset of bgm entries
                reader.BaseStream.Seek(startOffset, SeekOrigin.Begin);

                for (int i = 0; i < MaxEntries; i++)
                {
                    WAVEBANKENTRY entry;
                    int count = Marshal.SizeOf(typeof(WAVEBANKENTRY));
                    byte[] readBuffer;
                    readBuffer = reader.ReadBytes(count);

                    GCHandle handle = GCHandle.Alloc(readBuffer, GCHandleType.Pinned);
                    entry = (WAVEBANKENTRY)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(WAVEBANKENTRY));
                    entries[i] = entry;
                }
            }

            Output.Log(LogLevel.INFO, "Parsed XWB wave entries");

            return entries;
        }

        private static string[] WaveNames(string inputFile)
        {
            Output.Log(LogLevel.INFO, "Parsing XSB entry names");

            string xsbFile = $@"{Path.GetDirectoryName(inputFile)}\{Path.GetFileNameWithoutExtension(inputFile)}.xsb";
            if (!File.Exists(xsbFile))
            {
                throw new FileNotFoundException("Missing XSB file!", Path.GetFullPath(xsbFile));
            }

            string[] names = new string[MaxEntries];

            using (BinaryReader reader = new(File.OpenRead(xsbFile), Encoding.ASCII))
            {
                int startOffset = 0xbdd2;
                reader.BaseStream.Seek(startOffset, SeekOrigin.Begin);

                StringBuilder stringBuilder = new();
                for (int i = 0; i < MaxEntries; i++)
                {
                    switch (i)
                    {
                        case 855:
                        case 857:
                        case 1040:
                        case 1100:
                            names[i] = "Undefined";
                            break;

                        default:
                            while (reader.PeekChar() > -1)
                            {
                                stringBuilder.Append(reader.ReadChar());
                                if (reader.PeekChar() == 0)
                                {
                                    reader.ReadByte();
                                    break;
                                }
                            }

                            names[i] = stringBuilder.ToString();
                            stringBuilder.Clear();
                            break;
                    }
                }
            }

            Output.Log(LogLevel.INFO, "Parsed XSB entry names");

            return names;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WaveFile
        {
            private WaveFactChunk factChunk;

            public byte[] ChunkID { get => new byte[] { 0x52, 0x49, 0x46, 0x46 }; }

            public uint ChunkSize { get; init; }

            public byte[] Format { get => new byte[] { 0x57, 0x41, 0x56, 0x45 }; }

            public byte[] Subchunk1ID { get => new byte[] { 0x66, 0x6d, 0x74, 0x20 }; }

            public uint Subchunk1Size { get; init; }

            public ushort AudioFormat { get; init; }

            public ushort NumChannels { get; init; }

            public uint SampleRate { get; init; }

            public uint ByteRate { get; init; }

            public ushort BlockAlign { get; init; }

            public ushort BitsPerSample { get; init; }

            public WaveFactChunk FactChunk { get => this.factChunk; init => this.factChunk = new WaveFactChunk(this.BlockAlign, this.NumChannels, this.BitsPerSample, this.Subchunk2Size); }

            public byte[] Subchunk2ID { get => new byte[] { 0x64, 0x61, 0x74, 0x61 }; }

            public uint Subchunk2Size { get; init; }

            public byte[] Data { get; init; }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WaveFactChunk
        {
            public WaveFactChunk(ushort blockAlign, ushort numChannels, ushort bitsPerSample, uint dataChunkSize)
            {
                uint dw = (uint)((((uint)blockAlign - (7 * (uint)numChannels)) * 8) / (uint)bitsPerSample);
                dw = (dataChunkSize / blockAlign) * dw;
                this.UncompressedSize = dw / numChannels;
            }

            public byte[] ChunkID { get => new byte[] { 0x66, 0x61, 0x63, 0x74 }; }

            public uint ChunkSize { get => 4; }

            public uint UncompressedSize { get; init; }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WAVEBANKENTRY
        {
            public uint Flags1 { get; init; }

            public WAVEBANKMINIFORMAT Format { get; init; }

            public WAVEBANKEREGION PlayRegion { get; init; }

            public WAVEBANKLOOPREGION LoopRegion { get; init; }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WAVEBANKMINIFORMAT
        {
            public uint DwValue { get; set; }

            public uint FormatTag { get => this.DwValue & (uint)FormatMasks.FormatTagMask; }

            public uint Channels { get => (this.DwValue & (uint)FormatMasks.ChannelsMask) >> 2; }

            public uint SamplesPerSec { get => (this.DwValue & (uint)FormatMasks.SamplesPerSecMask) >> 5; }

            public uint BlockAlign { get => (this.DwValue & (uint)FormatMasks.BlockAlignMask) >> 23; }

            public uint BitsPerSample { get => (this.DwValue & (uint)FormatMasks.BitsPerSampleMask) >> 31; }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WAVEBANKLOOPREGION
        {
            public uint DwStartSamples { get; init; }

            public uint DwTotalSamples { get; init; }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WAVEBANKEREGION
        {
            public uint DwOffset { get; init; }

            public uint DwLength { get; init; }
        }
    }
}