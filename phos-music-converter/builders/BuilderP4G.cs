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
    /// Music Builder for Persona 4 Golden.
    /// </summary>
    internal class BuilderP4G : BuilderBase
    {
        private readonly string cachedDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuilderP4G"/> class.
        /// </summary>
        /// <param name="path">Path to Music Data JSON file.</param>
        /// <param name="verbose">Verbose setting for errors.</param>
        public BuilderP4G(string path, bool verbose)
            : base(path, "P4G", verbose)
        {
            this.cachedDirectory = $@"{Directory.GetCurrentDirectory()}\cached";
        }

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
            HashSet<string> uniqueSongs = new HashSet<string>();
            foreach (var song in this.GetMusicData().songs)
            {
                if (song.replacementFilePath != null)
                {
                    uniqueSongs.Add(song.replacementFilePath);
                }
            }

            Parallel.ForEach(uniqueSongs, song =>
            {
                this.EncodeSong(song, $@"{this.cachedDirectory}\{Path.GetFileNameWithoutExtension(song)}.raw");
            });
        }

        /// <summary>
        /// Encodes the file <paramref name="songPath"/> to .raw file at <paramref name="outPath"/>.
        /// Only encodes if the file <paramref name="songPath"/> has not been encoded before or has been edited since the last encoding.
        /// </summary>
        /// <param name="songPath">Path of file to encode.</param>
        /// <param name="outPath">Encoded output file path.</param>
        private void EncodeSong(string songPath, string outPath)
        {
            WaveProps waveProps = default(WaveProps);
            if (!Waves.LoadWaveProps(songPath, ref waveProps))
            {
                throw new ArgumentException("Problem reading the wave properties of file!", songPath);
            }

            /*
            // check if input file should be re-encoded
            bool waveRequiresEncoding = this.RequiresEncoding(songPath, outPath);

            // only update txth file if wave doesn't need to be encoded
            if (!waveRequiresEncoding)
            {
                Console.WriteLine("Updating txth file!");
                // store result of txth updated
                bool txthUpdated = txthHandler.UpdateTxthFile($"{outputFilePath}.txth", startSample, endSample);
                return txthUpdated;
            }

            // file path to store temp encoded file (still has header)
            string tempFilePath = $@"{outputFilePath}.temp";

            ProcessStartInfo encodeInfo = new ProcessStartInfo
            {
                FileName = encoderPath,
                Arguments = $@"""{inputFilePath}"" ""{tempFilePath}""",
            };

            // encode file given
            try
            {
                Process process = Process.Start(encodeInfo);
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    Console.WriteLine($"Problem with AdpcmEncode! Exit code: {process.ExitCode}");
                    return false;
                }
            }
            catch (Exception e)
            {
                // problem starting process, exit early
                Console.WriteLine("Problem running AdpcmEncode!");
                Console.WriteLine(e);
                return false;
            }

            // get wave props of input file
            WaveProps inputWaveProps = GetWaveProps(inputFilePath);
            // get num samples from input wave
            int numSamples = GetNumSamples(inputWaveProps);
            if (numSamples <= 0)
            {
                Console.WriteLine("Could not determine number of samples from input wave!");
                return false;
            }

            // get wave props of temp file
            WaveProps outputWaveProps = GetWaveProps(tempFilePath);

            // array to store data chunk bytes
            byte[] outDataChunk = new byte[outputWaveProps.Subchunk2Size];

            try
            {
                // read data chunk into array
                using (FileStream tempfile = File.OpenRead(tempFilePath))
                {
                    tempfile.Seek(outputWaveProps.DataOffset, SeekOrigin.Begin);
                    tempfile.Read(outDataChunk, 0, outDataChunk.Length);
                }
            }
            catch (Exception e)
            {
                // exit early if error reading data chunk
                Console.WriteLine($"Problem reading in data chunk of output!");
                Console.WriteLine(e);
                return false;
            }

            // write txth file
            bool txthSuccess = txthHandler.WriteTxthFile($"{outputFilePath}.txth", outputWaveProps, numSamples, startSample, endSample);
            if (!txthSuccess)
                return false;

            // write raw to file
            try
            {
                // write raw file
                File.WriteAllBytes($"{outputFilePath}", outDataChunk);
                // delete temp file
                File.Delete(tempFilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem writing raw to file!");
                Console.WriteLine(e);
                return false;
            }

            return true;
            */
        }
    }
}