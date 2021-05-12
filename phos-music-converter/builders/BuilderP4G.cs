// Copyright (c) T-Pose Ratkechi. All rights reserved.
// Licensed under the GNU GPLv3 license. See LICENSE file in the project root for full license information.

namespace PhosMusicConverter.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using PhosMusicConverter.Common;

    /// <summary>
    /// Music Builder for Persona 4 Golden.
    /// </summary>
    internal class BuilderP4G : BuilderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuilderP4G"/> class.
        /// </summary>
        /// <param name="path">Path to Music Data JSON file.</param>
        /// <param name="encoder">Path to encoder.</param>
        public BuilderP4G(string path, string encoder)
            : base("P4G", path, encoder)
        {
        }

        /// <inheritdoc/>
        protected override string CachedDirectory { get => $@"{Directory.GetCurrentDirectory()}\cached\adpcm"; }

        /// <inheritdoc/>
        public override void GenerateBuild(string outputDir, bool useLow)
        {
            // Check that the encoder exists.
            if (!File.Exists(this.EncoderPath))
            {
                throw new FileNotFoundException($"AdpcmEncode.exe could not be found!", Path.GetFullPath(this.EncoderPath));
            }

            // Encode unique files to cache.
            this.EncodeUniqueFiles();

            // Check the output directory exists, if not then create it.
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            // Empty out the output folder.
            foreach (var file in Directory.GetFiles(outputDir))
            {
                File.Delete(file);
            }

            int totalSongs = 0;

            // Copy from cache files to the proper destination.
            foreach (var song in this.GetMusicData().songs)
            {
                if (song.replacementFilePath != null && song.isEnabled)
                {
                    string cachedFileName = $"{Path.GetFileNameWithoutExtension(song.replacementFilePath)}.raw";

                    // copy cached raw and txth for song to output directory as the correct wave index
                    File.Copy($@"{this.CachedDirectory}\{cachedFileName}", $@"{outputDir}\{song.waveIndex}.raw");
                    File.Copy($@"{this.CachedDirectory}\{cachedFileName}.txth", $@"{outputDir}\{song.waveIndex}.raw.txth");
                    totalSongs++;
                }
            }

            Output.Log(LogLevel.INFO, $"Music Build generated with {totalSongs} total songs");
        }

        /// <summary>
        /// Iterates over local music data and encodes every unique replacement file.
        /// </summary>
        private void EncodeUniqueFiles()
        {
            HashSet<Song> uniqueSongs = new();
            foreach (var song in this.GetMusicData().songs)
            {
                if (song.replacementFilePath != null)
                {
                    uniqueSongs.Add(song);
                }
            }

            Output.Log(LogLevel.LOG, $"Processing {uniqueSongs.Count} songs");
            Parallel.ForEach(uniqueSongs, song =>
            {
                this.EncodeSong(song.replacementFilePath, $@"{this.CachedDirectory}\{Path.GetFileNameWithoutExtension(song.replacementFilePath)}.raw", song.loopStartSample, song.loopEndSample);
            });
            Output.Log(LogLevel.INFO, $"Processed {uniqueSongs.Count} songs");
        }

        /// <summary>
        /// Encodes the file <paramref name="songPath"/> to .raw file at <paramref name="outPath"/>.
        /// Only encodes if the file <paramref name="songPath"/> has not been encoded before or has been edited since the last encoding.
        /// </summary>
        /// <param name="songPath">Path of file to encode.</param>
        /// <param name="outPath">Encoded output file path.</param>
        /// <param name="startSample">Loop start sample.</param>
        /// <param name="endSample">Loop end sample.</param>
        private void EncodeSong(string songPath, string outPath, int startSample, int endSample)
        {
            // store file name for logging
            string fileName = Path.GetFileName(songPath);

            // check if input file should be re-encoded
            bool requiresEncoding = this.RequiresEncoding(songPath, outPath);

            // only update txth file if wave doesn't need to be encoded
            if (!requiresEncoding)
            {
                Output.Log(LogLevel.INFO, $"{fileName}: Using cached encoded file");
                TxthHandler.UpdateTxthFile($"{outPath}.txth", startSample, endSample);
                return;
            }

            // File path to store temp encoded file (still has header).
            string tempFilePath = $@"{outPath}.temp";

            ProcessStartInfo encodeInfo = new()
            {
                FileName = this.EncoderPath,
                Arguments = $@"""{songPath}"" ""{tempFilePath}""",
                CreateNoWindow = true,
            };

            Process encoder = Process.Start(encodeInfo);
            encoder.WaitForExit();

            // Load the required wave properties.
            WaveProps waveProps = default;
            if (!Waves.LoadWaveProps(songPath, ref waveProps))
            {
                throw new ArgumentException("Problem reading the wave properties of file!", songPath);
            }

            // Get total number of samples from input wave.
            int waveTotalSamples = Waves.GetTotalSamples(waveProps);
            if (waveTotalSamples == -1)
            {
                Output.Log(LogLevel.ERROR, "Failed to calculate total samples! Re-converting the file to wave can sometimes fix this.");
                return;
            }

            // Get wave props of temp file.
            WaveProps outputWaveProps = default;
            if (!Waves.LoadWaveProps(tempFilePath, ref outputWaveProps))
            {
                // Throw if wave props failed to load
                throw new ArgumentException("Problem reading the wave properties of temporary encoded file!", tempFilePath);
            }

            // Array to store data chunk bytes.
            byte[] outDataChunk = new byte[outputWaveProps.SubchunkSize2];

            // Read data chunk into array.
            using (FileStream tempfile = File.OpenRead(tempFilePath))
            {
                tempfile.Seek(outputWaveProps.DataStartOffset, SeekOrigin.Begin);
                tempfile.Read(outDataChunk, 0, outDataChunk.Length);
            }

            // Write txth file.
            TxthHandler.WriteTxthFile($"{outPath}.txth", outputWaveProps, waveTotalSamples, startSample, endSample);

            File.WriteAllBytes($"{outPath}", outDataChunk);

            // Delete temp file.
            File.Delete(tempFilePath);
        }
    }
}