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
        /// Verbose setting for errors.
        /// </summary>
        private readonly bool isVerbose;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuilderBase"/> class.
        /// </summary>
        /// <param name="musicDataPath">Path to music data JSON file.</param>
        /// <param name="gameName">Name of game the Music Builder is for.</param>
        /// <param name="verbose">Verbose setting for errors.</param>
        protected BuilderBase(string musicDataPath, string gameName, bool verbose)
        {
            this.isVerbose = verbose;
            this.musicData = MusicDataParser.ParseMusicData(musicDataPath);
            if (!Directory.Exists(this.CachedDirectory))
            {
                Directory.CreateDirectory(this.CachedDirectory);
            }
            Console.WriteLine($"{gameName} Music Builder");
        }

        /// <summary>
        /// Gets cache directory for game's Music Builds.
        /// </summary>
        protected abstract string CachedDirectory { get; }

        /// <inheritdoc/>
        public abstract void GenerateBuild(string outputDir, bool useLow, bool verbose);

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
            // outfile doesn't even exist
            if (!File.Exists(outfile))
            {
                return true;
            }

            try
            {
                // get currently saved checksum of file
                byte[] savedSum = ChecksumUtils.GetSavedChecksum(file, this.CachedDirectory);

                // file has no saved checksum so is new file
                if (savedSum == null)
                {
                    return true;
                }

                // check if file's current sum still matches saved sum
                if (savedSum.SequenceEqual(ChecksumUtils.GetChecksum(file)))
                {
                    // Console.WriteLine("Checksum match! Re-encoding not required...");
                    return false;
                }
                else
                {
                    // Console.WriteLine("Checksum mismatch! Re-encoding required!");
                    return true;
                }
            }
            catch (Exception e)
            {
                if (this.isVerbose)
                {
                    Console.WriteLine(e);
                }

                Console.WriteLine("[ERROR] Problem checking file checksums!");
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