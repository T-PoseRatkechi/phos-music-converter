// Copyright (c) T-Pose Ratkechi. All rights reserved.
// Licensed under the GNU GPLv3 license. See LICENSE file in the project root for full license information.

namespace PhosMusicConverter.Builders
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using PhosMusicConverter.Common;
    using PhosMusicConverter.Interfaces;

    /// <summary>
    /// Utility functions for batch stuff.
    /// </summary>
    internal class BatchBuilder
    {
        /// <summary>
        /// Convert a folder of wave files to a game's cached format.
        /// </summary>
        /// <param name="builder">Game music builder to encode files.</param>
        /// <param name="inputDir">Input directory containing waves.</param>
        /// <param name="useLow">Performance setting.</param>
        public static void Batch(IMusicBuilder builder, string inputDir, bool useLow)
        {
            Output.Log(LogLevel.INFO, "Batch Encoding");
            Output.Log(LogLevel.INFO, $"Folder: {inputDir}");

            string[] waveFiles = Directory.GetFiles(inputDir, "*.wav", SearchOption.TopDirectoryOnly);

            string outputDir = $@"{inputDir}\encoded";
            Directory.CreateDirectory(outputDir);

            // Empty out the output folder.
            foreach (var file in Directory.GetFiles(outputDir))
            {
                if (!Path.GetExtension(file).ToLower().Equals(".p4g"))
                {
                    File.Delete(file);
                }
            }

            // Encode every wave file to output.
            // TODO: Account for which game it is.
            if (!useLow)
            {
                Parallel.ForEach(waveFiles, song =>
                {
                    builder.EncodeSong(song, $@"{outputDir}\{Path.GetFileNameWithoutExtension(song)}.raw");
                });
            }
            else
            {
                foreach (var song in waveFiles)
                {
                    builder.EncodeSong(song, $@"{outputDir}\{Path.GetFileNameWithoutExtension(song)}.raw");
                }
            }

            Output.Log(LogLevel.INFO, $"Output: {outputDir}");
        }
    }
}