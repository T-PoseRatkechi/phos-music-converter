// Copyright (c) T-Pose Ratkechi. All rights reserved.
// Licensed under the GNU GPLv3 license. See LICENSE file in the project root for full license information.

namespace PhosMusicConverter.Interfaces
{
    /// <summary>
    /// Interface that all game Music Builders implement.
    /// </summary>
    internal interface IMusicBuilder
    {
        /// <summary>
        /// Generates a music build in <paramref name="outputDir"/>.
        /// </summary>
        /// <param name="outputDir">Directory to output music build to.</param>
        /// <param name="useLow">Whether to use less resource intensive processes for generating builds.</param>
        void GenerateBuild(string outputDir, bool useLow);

        /// <summary>
        /// Encodes the file <paramref name="songPath"/> to <paramref name="outPath"/>.
        /// Only encodes if the file <paramref name="songPath"/> has not been encoded before or has been edited since the last encoding.
        /// </summary>
        /// <param name="songPath">Path of file to encode.</param>
        /// <param name="outPath">Encoded output file path.</param>
        /// <param name="startSample">(Optional) Loop start sample.</param>
        /// <param name="endSample">(Optional) Loop end sample.</param>
        void EncodeSong(string songPath, string outPath, int startSample = 0, int endSample = 0);
    }
}