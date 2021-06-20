// Copyright (c) T-Pose Ratkechi. All rights reserved.
// Licensed under the GNU GPLv3 license. See LICENSE file in the project root for full license information.

namespace PhosMusicConverter.Builders
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using PhosMusicConverter.Common;

    /// <summary>
    /// Utility functions for export stuff.
    /// </summary>
    internal class ExportBuilder
    {
        /// <summary>
        /// Export a music build's encoded files.
        /// </summary>
        /// <param name="builder">Game music builder to encode files.</param>
        /// <param name="outputDir">Input directory containing waves.</param>
        /// <param name="useLow">Performance setting.</param>
        public static void Export(BuilderBase builder, string outputDir, bool useLow)
        {
            Output.Log(LogLevel.INFO, "Exporting Music Build");
            Output.Log(LogLevel.INFO, $"Folder: {outputDir}");

            Directory.CreateDirectory(outputDir);

            HashSet<Song> uniqueSongs = new(new UniqueSongsComparer());
            foreach (var song in builder.GetMusicData().songs)
            {
                // Add each unique replacement file in the music build, excluding already encoded .raw files.
                if (song.replacementFilePath != null)
                {
                    uniqueSongs.Add(song);
                }
            }

            // Encode every wave file to output.
            if (!useLow)
            {
                Parallel.ForEach(uniqueSongs, song =>
                {
                    builder.EncodeSong(song.replacementFilePath, $@"{outputDir}\{Path.GetFileNameWithoutExtension(song.replacementFilePath)}{builder.EncodedFileExt}");
                });
            }
            else
            {
                foreach (var song in uniqueSongs)
                {
                    builder.EncodeSong(song.replacementFilePath, $@"{outputDir}\{Path.GetFileNameWithoutExtension(song.replacementFilePath)}{builder.EncodedFileExt}");
                }
            }

            Output.Log(LogLevel.INFO, $"Output: {uniqueSongs.Count} files to {outputDir}");
        }
    }
}