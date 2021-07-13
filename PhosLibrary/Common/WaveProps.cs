// Copyright (c) T-Pose Ratkechi. All rights reserved.
// Licensed under the GNU GPLv3 license. See LICENSE file in the project root for full license information.

namespace PhosLibrary.Common
{
    /// <summary>
    /// Struct containing only the necessary information needed about waves for all of this to work.
    /// </summary>
    public readonly struct WaveProps
    {
        /// <summary>
        /// Gets the number of channels.
        /// </summary>
        public readonly short NumChannels { get; init; }

        /// <summary>
        /// Gets the sample rate.
        /// </summary>
        public readonly int SampleRate { get; init; }

        /// <summary>
        /// Gets wave block alignment.
        /// </summary>
        public readonly short BlockAlign { get; init; }

        /// <summary>
        /// Gets bits per samples.
        /// </summary>
        public readonly short BitsPerSamples { get; init; }

        /// <summary>
        /// Gets byte array containing extra params. Null if files has no extra params.
        /// </summary>
        public readonly byte[] ExtraParamsBytes { get; init; }

        /// <summary>
        /// Gets the size of Subchunk 2.
        /// </summary>
        public readonly int SubchunkSize2 { get; init; }

        /// <summary>
        /// Gets the starting offset of the data chunk.
        /// </summary>
        public readonly long DataStartOffset { get; init; }
    }
}