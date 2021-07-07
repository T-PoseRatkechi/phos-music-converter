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
        /// <param name="musicDataPath">Path of music data to export.</param>
        /// <param name="builder">Game music builder to encode files.</param>
        /// <param name="outputDir">Input directory containing waves.</param>
        /// <param name="useLow">Performance setting.</param>
        public static void Export(string musicDataPath, BuilderBase builder, string outputDir, bool useLow)
        {
            Output.Log(LogLevel.INFO, "Exporting Music Build");
            Output.Log(LogLevel.INFO, $"Folder: {outputDir}");

            MusicData originalMusicData = MusicDataParser.Parse(musicDataPath);

            HashSet<Song> uniqueSongs = new(new UniqueSongsComparer());
            foreach (var song in originalMusicData.songs)
            {
                // Add each unique replacement file in the music build.
                if (song.replacementFilePath != null && song.isEnabled)
                {
                    uniqueSongs.Add(song);
                }
            }

            // Create mock music data just with unique files.
            List<Song> adjustedSongs = new();
            foreach (var song in uniqueSongs)
            {
                // Adjusted output file path for song to export folder and as encoded.
                string newOutputFilePath = $@"\{Path.GetFileNameWithoutExtension(song.replacementFilePath)}{builder.EncodedFileExt}";

                adjustedSongs.Add(new Song()
                {
                    id = null,
                    isEnabled = true,
                    name = null,
                    originalFile = null,
                    replacementFilePath = song.replacementFilePath,
                    loopStartSample = song.loopStartSample,
                    loopEndSample = song.loopEndSample,
                    outputFilePath = newOutputFilePath,
                    extraData = song.extraData,
                });
            }

            MusicData mockMusicData = new() { songs = adjustedSongs.ToArray() };

            // Build mock music data.
            builder.GenerateBuild(mockMusicData, outputDir, useLow);

            Output.Log(LogLevel.INFO, $"Exported {uniqueSongs.Count} files to {outputDir}");
        }
    }
}