// Copyright (c) T-Pose Ratkechi. All rights reserved.
// Licensed under the GNU GPLv3 license. See LICENSE file in the project root for full license information.

namespace PhosLibrary.Common
{
    using System;
    using System.IO;
    using System.Linq;
    using PhosLibrary.Common.Logging;

    /// <summary>
    /// Class containing required functions related to wave files.
    /// </summary>
    internal class Waves
    {
        /// <summary>
        /// Loads the useful properties of <paramref name="wavefile"/> into <paramref name="props"/>.
        /// </summary>
        /// <param name="wavefile">Path to the wave file.</param>
        /// <param name="props">UsefulProps instance to load <paramref name="props"/>'s properties.</param>
        /// <returns>Whether the <c>WaveProps</c> were loaded successfully into <paramref name="props"/>.</returns>
        public static bool LoadWaveProps(string wavefile, ref WaveProps props)
        {
            using (BinaryReader reader = new(File.OpenRead(wavefile)))
            {
                // Check RIFF header
                if (!reader.ReadBytes(4).SequenceEqual(new byte[] { 0x52, 0x49, 0x46, 0x46 }))
                {
                    Output.Log(LogLevel.ERROR, "Missing RIFF header!");
                    return false;
                }

                // Skip ChunkSize
                reader.BaseStream.Seek(4, SeekOrigin.Current);

                // Check WAVE format
                if (!reader.ReadBytes(4).SequenceEqual(new byte[] { 0x57, 0x41, 0x56, 0x45 }))
                {
                    Output.Log(LogLevel.ERROR, "Format not WAVE!");
                    return false;
                }

                // Check "fmt " bytes
                if (!reader.ReadBytes(4).SequenceEqual(new byte[] { 0x66, 0x6d, 0x74, 0x20 }))
                {
                    Output.Log(LogLevel.ERROR, "Missing \"fmt \" bytes!");
                    return false;
                }

                // Save subchunkSize1 for later
                int subchunkSize1 = reader.ReadInt32();

                // Skip AudioFormat bytes
                reader.BaseStream.Seek(2, SeekOrigin.Current);

                // Save number of channels for later
                short numChannels = reader.ReadInt16();

                // Save sample rate for later
                int sampleRate = reader.ReadInt32();

                // Skip ByteRate bytes
                reader.BaseStream.Seek(4, SeekOrigin.Current);

                // Save block align for later
                short blockAlign = reader.ReadInt16();

                // Save bits per sample (used to get total samples)
                short bitsPerSamples = reader.ReadInt16();

                short extraParamsSize = 0;
                byte[] extraParamBytes = null;

                // Read extra params if subchunkSize1 > 16
                if (subchunkSize1 > 16)
                {
                    extraParamsSize = reader.ReadInt16();
                    extraParamBytes = reader.ReadBytes(extraParamsSize);
                }

                // Skip subchunk2ID bytes
                reader.BaseStream.Seek(4, SeekOrigin.Current);

                // Save subchunk2Size for later
                int subchunk2Size = reader.ReadInt32();

                // Save start offset of wave data
                long dataStartOffset = reader.BaseStream.Position;

                props = new WaveProps()
                {
                    NumChannels = numChannels,
                    SampleRate = sampleRate,
                    BlockAlign = blockAlign,
                    BitsPerSamples = bitsPerSamples,
                    ExtraParamsBytes = extraParamBytes,
                    SubchunkSize2 = subchunk2Size,
                    DataStartOffset = dataStartOffset,
                };
            }

            return true;
        }

        /// <summary>
        /// Calculates and returns the total number of samples of the given <paramref name="props"/>.
        /// </summary>
        /// <param name="props">The wave file's <c>WaveProps</c>.</param>
        /// <returns>The total number of samples. Returns -1 if a <c>DivideByZeroException</c> occurs.</returns>
        public static int GetTotalSamples(WaveProps props)
        {
            try
            {
                int totalSamples = props.SubchunkSize2 / props.NumChannels / (props.BitsPerSamples / 8);
                return totalSamples;
            }
            catch (DivideByZeroException)
            {
                return -1;
            }
        }
    }
}