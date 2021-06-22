// Copyright (c) T-Pose Ratkechi. All rights reserved.
// Licensed under the GNU GPLv3 license. See LICENSE file in the project root for full license information.

namespace PhosMusicConverter.Builders
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using PhosMusicConverter.Common;

    /// <summary>
    /// Base class for Music Builders.
    /// </summary>
    internal abstract class BuilderBase
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
            // Parse music data file if given.
            if (musicDataPath != null)
            {
                this.musicData = MusicDataParser.ParseMusicData(musicDataPath);
            }

            this.EncoderPath = encoder;
            if (!Directory.Exists(this.CachedDirectory))
            {
                Directory.CreateDirectory(this.CachedDirectory);
            }

            Output.Log(LogLevel.INFO, $"Using {gameName} Music Builder");
        }

        /// <summary>
        /// Gets supported input formats for builder.
        /// </summary>
        public abstract string[] SupportedFormats { get; }

        /// <summary>
        /// Gets file extension for file encoded by the builder's encoder.
        /// </summary>
        public abstract string EncodedFileExt { get; }

        /// <summary>
        /// Gets path to encoder.
        /// </summary>
        protected string EncoderPath { get; init; }

        /// <summary>
        /// Gets cache directory for game's Music Builds.
        /// </summary>
        protected abstract string CachedDirectory { get; }

        /// <summary>
        /// Generates a music build in <paramref name="outputDir"/>.
        /// </summary>
        /// <param name="outputDir">Directory to output music build to.</param>
        /// <param name="useLow">Whether to use less resource intensive processes for generating builds.</param>
        public virtual void GenerateBuild(string outputDir, bool useLow)
        {
            // Check that the encoder exists.
            if (!File.Exists(this.EncoderPath))
            {
                throw new FileNotFoundException($"{Path.GetFileName(this.EncoderPath)} could not be found!", Path.GetFullPath(this.EncoderPath));
            }

            // Check the output directory exists, if not then create it.
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
                Output.Log(LogLevel.DEBUG, $"Created output directory: {outputDir}");
            }

            string[] outputFiles = Directory.GetFiles(outputDir, "*.*", SearchOption.AllDirectories);
            if (outputFiles.Length > 100)
            {
                throw new ArgumentException("Output directory has an unusually large amount of files! Caution!");
            }

            // Empty out the output folder.
            foreach (var file in outputFiles)
            {
                File.Delete(file);
            }

            this.BuildDirectories(outputDir);
            this.BuildCache(useLow);
            this.BuildOutput(outputDir, useLow);
        }

        private void BuildDirectories(string outputDir)
        {
            HashSet<string> uniqueDirectories = new();

            foreach (var song in this.GetMusicData().songs)
            {
                string outputFile = $"{outputDir}{song.outputFilePath}";
                string songDirectory = Path.GetDirectoryName(outputFile);
                uniqueDirectories.Add(songDirectory);
            }

            foreach (var songDir in uniqueDirectories)
            {
                if (!Directory.Exists(songDir))
                {
                    Directory.CreateDirectory(songDir);
                    Output.Log(LogLevel.DEBUG, $"Created sub-directory: {songDir}");
                }
            }
        }

        /// <summary>
        /// Encodes <paramref name="songPath"/> using builder's encoder and with the specified loop points. Will only
        /// encode the file if an encoded copy is not already cached or <paramref name="songPath"/> has been modified
        /// since then. The encoded file is then copied to <paramref name="outPath"/>, if specified.
        /// </summary>
        /// <param name="songPath">Path to song file to encode.</param>
        /// <param name="outPath">Path to output encoded file.</param>
        /// <param name="startSample">Loop start sample.</param>
        /// <param name="endSample">Loop end sample.</param>
        public virtual void EncodeSong(string songPath, string outPath = null, int startSample = 0, int endSample = 0)
        {
            string cachedSongPath = this.CachedFilePath(songPath);

            if (!this.RequiresEncoding(songPath, cachedSongPath))
            {
                this.ProcessEncodedSong(cachedSongPath, startSample, endSample);
                if (outPath != null)
                {
                    this.CopyFromCached(songPath, outPath, startSample, endSample);
                }
            }
            else
            {
                this.Encode(songPath, cachedSongPath, startSample, endSample);
                this.ProcessEncodedSong(cachedSongPath, startSample, endSample);

                if (outPath != null)
                {
                    this.CopyFromCached(songPath, outPath, startSample, endSample);
                }
            }
        }

        /// <summary>
        /// Get parsed Music Data object.
        /// </summary>
        /// <returns>Builder's music data.</returns>
        public MusicData GetMusicData()
        {
            return this.musicData;
        }

        /// <summary>
        /// Process an encoded file, if needed.
        /// </summary>
        /// <param name="encodedSong">Path to encoded file.</param>
        /// <param name="startSample">Loop start sample.</param>
        /// <param name="endSample">Loop end sample.</param>
        protected abstract void ProcessEncodedSong(string encodedSong, int startSample = 0, int endSample = 0);

        /// <summary>
        /// Handles copying files to output for already encoded files.
        /// </summary>
        /// <param name="encodedFile">Path to already encoded file.</param>
        /// <param name="outputPath">Output path.</param>
        /// <param name="startSample">Loop start sample.</param>
        /// <param name="endSample">Loop end sample.</param>
        protected virtual void CopyFromEncoded(string encodedFile, string outputPath, int startSample = 0, int endSample = 0)
        {
            // Copy already encoded file to output.
            File.Copy(encodedFile, outputPath, true);
        }

        /// <summary>
        /// Handles copying files to output from cached.
        /// </summary>
        /// <param name="songPath">Input song file path.</param>
        /// <param name="outputPath">Output file path.</param>
        /// <param name="startSample">Loop start sample.</param>
        /// <param name="endSample">Loop end sample.</param>
        protected virtual void CopyFromCached(string songPath, string outputPath, int startSample = 0, int endSample = 0)
        {
            // Copy cached encoded song to output.
            File.Copy(this.CachedFilePath(songPath), outputPath, true);
        }

        /// <summary>
        /// Gets the expected cached encoded file path for the given <paramref name="file"/>.
        /// </summary>
        /// <param name="file">Input file.</param>
        /// <returns>Cached file path for <paramref name="file"/>.</returns>
        protected string CachedFilePath(string file)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            return $@"{this.CachedDirectory}\{fileName}{this.EncodedFileExt}";
        }

        /// <summary>
        /// Encodes the <paramref name="inputFile"/> using encoder to <paramref name="outputFile"/>.
        /// </summary>
        /// <param name="inputFile">File to encode.</param>
        /// <param name="outputFile">Encoded output file.</param>
        /// <param name="startSample">Loop start sample.</param>
        /// <param name="endSample">Loop end sample.</param>
        protected abstract void Encode(string inputFile, string outputFile, int startSample = 0, int endSample = 0);

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
        /// Iterates over the builder's Music Data and encodes and caches every replacement file in use.
        /// </summary>
        /// <param name="useLow">Use low performance mode.</param>
        private void BuildCache(bool useLow)
        {
            Output.Log(LogLevel.INFO, "Building cache");

            HashSet<Song> uniqueSongs = new(new UniqueSongsComparer());
            foreach (var song in this.GetMusicData().songs)
            {
                // Add each unique replacement file in the music build, excluding already encoded .raw files.
                if (song.replacementFilePath != null)
                {
                    if (this.FileIsSupported(song.replacementFilePath))
                    {
                        uniqueSongs.Add(song);
                    }
                }
            }

            Output.Log(LogLevel.LOG, $"Processing {uniqueSongs.Count} songs");

            if (!useLow)
            {
                Parallel.ForEach(uniqueSongs, song =>
                {
                    this.EncodeSong(song.replacementFilePath, null, song.loopStartSample, song.loopEndSample);
                });
            }
            else
            {
                foreach (var song in uniqueSongs)
                {
                    this.EncodeSong(song.replacementFilePath, null, song.loopStartSample, song.loopEndSample);
                }
            }

            Output.Log(LogLevel.INFO, $"Processed {uniqueSongs.Count} songs");
        }

        /// <summary>
        /// Output music build.
        /// </summary>
        /// <param name="outputDir">Directory to output files to.</param>
        /// <param name="useLow">Use low performance mode.</param>
        private void BuildOutput(string outputDir, bool useLow)
        {
            int totalSongs = 0;

            if (!useLow)
            {
                Parallel.ForEach(this.GetMusicData().songs, song =>
                {
                    if (song.replacementFilePath != null && song.isEnabled)
                    {
                        // Copy from cache encoded replacement file to build.
                        if (!this.FileIsEncoded(song.replacementFilePath))
                        {
                            this.CopyFromCached(song.replacementFilePath, $@"{outputDir}\{song.outputFilePath}", song.loopStartSample, song.loopEndSample);
                        }

                        // Copy already encoded file.
                        else if (this.FileIsEncoded(song.replacementFilePath))
                        {
                            // Copy the already encoded selected file to build.
                            this.CopyFromEncoded(song.replacementFilePath, $@"{outputDir}\{song.outputFilePath}", song.loopStartSample, song.loopEndSample);
                        }
                        else
                        {
                            throw new ArgumentException($"Unknown file type! File: {song.replacementFilePath}");
                        }

                        // Increment total songs in build.
                        totalSongs++;
                    }
                });
            }
            // TODO: Update sync method
            else
            {
                Output.Log(LogLevel.INFO, "Low performance mode enabled");

                // Copy from cache files to the proper destination.
                foreach (var song in this.GetMusicData().songs)
                {
                    /*
                    // Copy to build replaced songs that are enabled.
                    if (song.replacementFilePath != null && song.isEnabled)
                    {
                        // Copy from cache encoded replacement file to build.
                        if (!Path.GetExtension(song.replacementFilePath).ToLower().Equals(".raw"))
                        {
                            string cachedFileName = $"{Path.GetFileNameWithoutExtension(song.replacementFilePath)}.raw";

                            // Copy cached raw and txth for song to output directory as the correct wave index.
                            File.Copy($@"{this.CachedDirectory}\{cachedFileName}", $@"{outputDir}\{song.outputFilePath}");
                            File.Copy($@"{this.CachedDirectory}\{cachedFileName}.txth", $@"{outputDir}\{song.outputFilePath}.txth");
                        }
                        else
                        {
                            // Copy the already encoded selected file to build.
                            File.Copy($"{song.replacementFilePath}", $@"{outputDir}\{song.waveIndex}.raw");
                            File.Copy($"{song.replacementFilePath}.txth", $@"{outputDir}\{song.waveIndex}.raw.txth");

                            // Update the copied txth's loop samples to the song's given loop samples.
                            TxthHandler.UpdateTxthFile($@"{outputDir}\{song.waveIndex}.raw.txth", song.loopStartSample, song.loopEndSample);
                        }

                        // Increment total songs in build.
                        totalSongs++;
                    }
                    */
                }
            }

            Output.Log(LogLevel.INFO, $"Music Build generated with {totalSongs} total songs");
        }

        /// <summary>
        /// Check whether <paramref name="file"/> is already encoded to builder's encoded format.
        /// </summary>
        /// <param name="file">File to check.</param>
        /// <returns>Whether <paramref name="file"/> is already in builder's encoded format.</returns>
        private bool FileIsEncoded(string file)
        {
            return Path.GetExtension(file).ToLower().Equals(this.EncodedFileExt.ToLower());
        }

        /// <summary>
        /// Check if <paramref name="file"/> is of a supported type by builder.
        /// </summary>
        /// <param name="file">File to check.</param>
        /// <returns>Whether builder supports encoding <paramref name="file"/>.</returns>
        private bool FileIsSupported(string file)
        {
            string fileType = Path.GetExtension(file);
            return Array.FindIndex(this.SupportedFormats, x => x.ToLower().Equals(fileType.ToLower())) > -1;
        }
    }
}