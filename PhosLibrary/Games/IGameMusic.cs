// Copyright (c) T-Pose Ratkechi. All rights reserved.
// Licensed under the GNU GPLv3 license. See LICENSE file in the project root for full license information.

namespace PhosLibrary.Games
{
    using PhosLibrary.Common.MusicData;

    /// <summary>
    /// Interface for Game Music items with the 4 core methods.
    /// </summary>
    public interface IGameMusic
    {
        /// <summary>
        /// Generates a music build of <paramref name="musicData"/> at <paramref name="outputDir"/>.
        /// </summary>
        /// <param name="musicData">Music data to build.</param>
        /// <param name="outputDir">Folder to output build.</param>
        /// <param name="useLow">Low performance flag.</param>
        public void Build(MusicData musicData, string outputDir, bool useLow);

        /// <summary>
        /// Exports an encoded copy of every song used in <paramref name="musicData"/> at <paramref name="outputDir"/>.
        /// </summary>
        /// <param name="musicData">Music data to export.</param>
        /// <param name="outputDir">Folder to export to.</param>
        /// <param name="useLow">Low performance flag.</param>
        public void Export(MusicData musicData, string outputDir, bool useLow);

        /// <summary>
        /// Encodes supported files in <paramref name="inputDir"/>. Encoded files are outputted to a folder named "encoded".
        /// </summary>
        /// <param name="inputDir">Folder directory containing the files to encode.</param>
        /// <param name="useLow">Low performance flag.</param>
        public void Batch(string inputDir, bool useLow);

        /// <summary>
        /// Extracts supported songs from <paramref name="inputFile"/> into <paramref name="outputDir"/>.
        /// </summary>
        /// <param name="inputFile">Input game file to extract music from.</param>
        /// <param name="outputDir">Folder to output extracted music.</param>
        public void Extract(string inputFile, string outputDir);
    }
}