// Copyright (c) T-Pose Ratkechi. All rights reserved.
// Licensed under the GNU GPLv3 license. See LICENSE file in the project root for full license information.

namespace PhosMusicConverter.Builders
{
    using System;
    using System.IO;
    using System.Linq;
    using PhosMusicConverter.Common;
    using PhosMusicConverter.Interfaces;

    /// <summary>
    /// Base class for Music Builders.
    /// </summary>
    internal abstract class BuilderBase : IMusicBuilder
    {
        /// <summary>
        /// Parsed Music Data object.
        /// </summary>
        private readonly MusicData musicData;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuilderBase"/> class.
        /// </summary>
        /// <param name="gameName">Name of game the Music Builder is for.</param>
        /// <param name="musicDataPath">Path to music data JSON file.</param>
        /// <param name="encoder">Path of encoder to use.</param>
        protected BuilderBase(string gameName, string musicDataPath, string encoder)
        {
            this.musicData = MusicDataParser.ParseMusicData(musicDataPath);
            this.EncoderPath = encoder;
            if (!Directory.Exists(this.CachedDirectory))
            {
                Directory.CreateDirectory(this.CachedDirectory);
            }

            Output.Log(LogLevel.INFO, $"Using {gameName} Music Builder");
        }

        /// <summary>
        /// Gets cache directory for game's Music Builds.
        /// </summary>
        protected abstract string CachedDirectory { get; }

        /// <summary>
        /// Gets path to encoder.
        /// </summary>
        protected string EncoderPath { get; init; }

        /// <inheritdoc/>
        public abstract void GenerateBuild(string outputDir, bool useLow);

        /// <summary>
        /// Determines if <paramref name="file"/> should be encoded.
        /// Checks if <paramref name="file"/> has been encoded before, whether it has been cached,
        /// and whether <paramref name="file"/> has been edited since it was originally cached.
        /// </summary>
        /// <param name="file">File to determine if it requires encoding.</param>
        /// <param name="outfile">Expected output file of the encoded <paramref name="file"/>.</param>
        /// <returns>Whether <paramref name="file"/> should be encoded.</returns>
        protected bool RequiresEncoding(string file, string outfile)
        {
            // Get currently saved checksum of file.
            byte[] savedSum = ChecksumUtils.GetSavedChecksum(file, this.CachedDirectory);

            // Outfile doesn't even exist.
            if (!File.Exists(outfile))
            {
                Output.Log(LogLevel.DEBUG, $"Encoded .raw file missing. File wil be encoded. File: {file}");
                return true;
            }

            try
            {
                // File had no saved checksum, meaning it's a new file.
                if (savedSum == null)
                {
                    Output.Log(LogLevel.DEBUG, $"New file. File Will be encoded File: {file}");
                    return true;
                }

                // Check if file's current sum still matches saved sum.
                if (savedSum.SequenceEqual(ChecksumUtils.GetChecksum(file)))
                {
                    Output.Log(LogLevel.DEBUG, $"Saved checksum matches file. Encoding not required. File: {file}");
                    return false;
                }
                else
                {
                    Output.Log(LogLevel.DEBUG, $"Saved checksum doesn't match file. Re-encoding required. File: {file}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Output.Log(LogLevel.ERROR, ex.ToString());
                Output.Log(LogLevel.ERROR, "Problem checking file checksums!");
                return true;
            }
        }

        /// <summary>
        /// Get parsed Music Data object.
        /// </summary>
        /// <returns>Music Data.</returns>
        protected MusicData GetMusicData()
        {
            return this.musicData;
        }
    }
}