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
        private readonly string encoderPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuilderP4G"/> class.
        /// </summary>
        /// <param name="path">Path to Music Data JSON file.</param>
        /// <param name="verbose">Verbose setting for errors.</param>
        public BuilderP4G(string path, bool verbose)
            : base(path, "P4G", verbose)
        {
            this.encoderPath = $@"{Directory.GetCurrentDirectory()}\..\xacttool_0.1\tools\AdpcmEncode.exe";
        }

        /// <inheritdoc/>
        protected override string CachedDirectory { get => $@"{Directory.GetCurrentDirectory()}\cached\adpcm"; }

        /// <inheritdoc/>
        public override void GenerateBuild(string outputDir, bool useLow, bool verbose)
        {
            string currentDir = Directory.GetCurrentDirectory();

            string encoderPath = $@"{currentDir}\..\xacttool_0.1\tools\AdpcmEncode.exe";
            if (!File.Exists(encoderPath))
            {
                throw new FileNotFoundException($"AdpcmEncode.exe could not be found!", Path.GetFullPath(encoderPath));
            }

            this.EncodeUniqueFiles();
        }

        /// <summary>
        /// Iterates over local music data and encodes every unique replacement file.
        /// </summary>
        private void EncodeUniqueFiles()
        {
            HashSet<Song> uniqueSongs = new HashSet<Song>();
            foreach (var song in this.GetMusicData().songs)
            {
                if (song.replacementFilePath != null)
                {
                    uniqueSongs.Add(song);
                }
            }

            Parallel.ForEach(uniqueSongs, song =>
            {
                this.EncodeSong(song.replacementFilePath, $@"{this.CachedDirectory}\{Path.GetFileNameWithoutExtension(song.replacementFilePath)}.raw", song.loopStartSample, song.loopEndSample);
            });
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
            // check if input file should be re-encoded
            bool requiresEncoding = this.RequiresEncoding(songPath, outPath);

            // only update txth file if wave doesn't need to be encoded
            if (!requiresEncoding)
            {
                Console.WriteLine("Updating txth file!");

                // store result of txth updated
                TxthHandler.UpdateTxthFile($"{outPath}.txth", startSample, endSample);
                return;
            }

            // file path to store temp encoded file (still has header)
            string tempFilePath = $@"{outPath}.temp";

            ProcessStartInfo encodeInfo = new ProcessStartInfo
            {
                FileName = this.encoderPath,
                Arguments = $@"""{songPath}"" ""{tempFilePath}""",
            };

            Process process = Process.Start(encodeInfo);
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                Console.WriteLine($"Problem with AdpcmEncode! Exit code: {process.ExitCode}");
                return;
            }

            Console.WriteLine($"Output: {tempFilePath}");

            // Load the required wave properties
            WaveProps waveProps = default(WaveProps);
            if (!Waves.LoadWaveProps(songPath, ref waveProps))
            {
                // Throw if wave props failed to load
                // throw new ArgumentException("Problem reading the wave properties of file!", songPath);
            }

            // Get total number of samples from input wave
            int waveTotalSamples = Waves.GetTotalSamples(waveProps);
            if (waveTotalSamples == -1)
            {
                Console.WriteLine("[ERROR] Failed to calculate total samples! Re-converting the file to wave can sometimes fix this.");
                return;
            }

            // Get wave props of temp file
            WaveProps outputWaveProps = default(WaveProps);
            if (!Waves.LoadWaveProps(tempFilePath, ref outputWaveProps))
            {
                // Throw if wave props failed to load
                throw new ArgumentException("Problem reading the wave properties of temporary encoded file!", tempFilePath);
            }

            // Array to store data chunk bytes
            byte[] outDataChunk = new byte[outputWaveProps.SubchunkSize2];

            // read data chunk into array
            using (FileStream tempfile = File.OpenRead(tempFilePath))
            {
                tempfile.Seek(outputWaveProps.DataStartOffset, SeekOrigin.Begin);
                tempfile.Read(outDataChunk, 0, outDataChunk.Length);
            }

            // write txth file
            TxthHandler.WriteTxthFile($"{outPath}.txth", outputWaveProps, waveTotalSamples, startSample, endSample);

            File.WriteAllBytes($"{outPath}", outDataChunk);

            // delete temp file
            File.Delete(tempFilePath);
        }
    }
}