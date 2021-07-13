// Copyright (c) T-Pose Ratkechi. All rights reserved.
// Licensed under the GNU GPLv3 license. See LICENSE file in the project root for full license information.

namespace PhosLibrary.Games
{
    using System;
    using System.IO;
    using PhosLibrary.Builders;
    using PhosLibrary.Builders.Music;
    using PhosLibrary.Common.Extracting;
    using PhosLibrary.Common.Logging;
    using PhosLibrary.Common.MusicData;

    /// <summary>
    /// Base game music class that implements IGameMusic.
    /// </summary>
    public class GameMusicBase : IGameMusic
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameMusicBase"/> class.
        /// </summary>
        /// <param name="builder">MusicBuilder to use.</param>
        protected GameMusicBase(MusicBuilder builder)
        {
            this.MusicBuilder = builder;
        }

        private MusicBuilder MusicBuilder { get; init; }

        /// <inheritdoc/>
        public void Build(MusicData musicData, string outputDir, bool useLow)
        {
            this.MusicBuilder.GenerateBuild(musicData, outputDir, useLow);
        }

        /// <inheritdoc/>
        public void Batch(string inputDir, bool useLow)
        {
            BatchBuilder.Batch(this.MusicBuilder, inputDir, useLow);
        }

        /// <inheritdoc/>
        public void Export(MusicData musicData, string outputDir, bool useLow)
        {
            ExportBuilder.Export(musicData, this.MusicBuilder, outputDir, useLow);
        }

        /// <inheritdoc/>
        public void Extract(string inputFile, string outputDir)
        {
            try
            {
                string inputFileType = Path.GetExtension(inputFile).ToLower();
                switch (inputFileType)
                {
                    case ".xwb":
                        Output.Log(LogLevel.INFO, "Extracting music from XWB");
                        Output.Log(LogLevel.LOG, "Uses code from unxwb by Luigi Auriemma.");
                        Output.Log(LogLevel.LOG, "Licensed under the GNU GPLv3 license. See unxwb.LICENSE file in the project root for full license information.");
                        XwbUtils.ExtractSongs(inputFile, outputDir);
                        Output.Log(LogLevel.INFO, "Finished extracting music from XWB");
                        break;

                    default:
                        Output.Log(LogLevel.ERROR, $"Unrecognized file type: {inputFileType}");
                        break;
                }
            }
            catch (FileNotFoundException ex)
            {
                Output.Log(LogLevel.ERROR, ex.ToString());
                Output.Log(LogLevel.ERROR, $"Could not find file: {ex.FileName}");
            }
            catch (Exception ex)
            {
                Output.Log(LogLevel.ERROR, ex.ToString());
                Output.Log(LogLevel.ERROR, "Failed to extract music!");
            }
        }
    }
}