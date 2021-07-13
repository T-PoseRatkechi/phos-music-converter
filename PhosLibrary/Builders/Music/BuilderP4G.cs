// Copyright (c) T-Pose Ratkechi. All rights reserved.
// Licensed under the GNU GPLv3 license. See LICENSE file in the project root for full license information.

namespace PhosLibrary.Builders.Music
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using PhosLibrary.Common;
    using PhosLibrary.Common.Logging;

    /// <summary>
    /// Music Builder for Persona 4 Golden.
    /// </summary>
    internal class BuilderP4G : MusicBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuilderP4G"/> class.
        /// </summary>
        public BuilderP4G()
            : base("P4G")
        {
        }

        /// <inheritdoc/>
        public override string EncodedFileExt { get => ".raw"; }

        /// <inheritdoc/>
        public override string[] SupportedFormats { get => new string[] { ".wav" }; }

        /// <inheritdoc/>
        protected override string CachedDirectory { get => $@"{Directory.GetCurrentDirectory()}\cached\adpcm"; }

        /// <inheritdoc/>
        protected override string EncoderPath { get; } = Encoders.GetAdpcmPath();

        /// <inheritdoc/>
        protected override void CopyFromEncoded(string encodedPath, string outPath, int startSample = 0, int endSample = 0)
        {
            string fileName = Path.GetFileName(encodedPath);
            Output.Log(LogLevel.INFO, $"{fileName}: Using already encoded file");

            base.CopyFromEncoded(encodedPath, outPath);

            // Copy encoded file's txth to output.
            File.Copy($"{encodedPath}.txth", $"{outPath}.txth");

            TxthHandler.UpdateTxthFile($"{outPath}.txth", startSample, endSample);
        }

        /// <inheritdoc/>
        protected override void CopyFromCached(string songPath, string outPath, int startSample = 0, int endSample = 0)
        {
            string fileName = Path.GetFileName(songPath);
            Output.Log(LogLevel.INFO, $"{fileName}: Using cached encoded file");

            base.CopyFromCached(songPath, outPath);

            string cachedTxthPath = $@"{this.CachedFilePath(songPath)}.txth";
            File.Copy(cachedTxthPath, $"{outPath}.txth", true);
        }

        /// <inheritdoc/>
        protected override void Encode(string inputFile, string outputFile, int startSample = 0, int endSample = 0)
        {
            // File path to store temp encoded file (still has header).
            string tempFilePath = $@"{outputFile}.temp";

            ProcessStartInfo encodeInfo = new()
            {
                FileName = this.EncoderPath,
                Arguments = $@"""{inputFile}"" ""{tempFilePath}""",
                CreateNoWindow = true,
            };

            Process encoder = Process.Start(encodeInfo);
            encoder.WaitForExit();

            // Load the required wave properties.
            WaveProps waveProps = default;
            if (!Waves.LoadWaveProps(inputFile, ref waveProps))
            {
                throw new ArgumentException("Problem reading the wave properties of file!", inputFile);
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
            TxthHandler.WriteTxthFile($"{outputFile}.txth", outputWaveProps, waveTotalSamples, startSample, endSample);

            File.WriteAllBytes($"{outputFile}", outDataChunk);

            // Delete temp file.
            File.Delete(tempFilePath);
        }

        /// <inheritdoc/>
        protected override void ProcessEncodedSong(string encodedSong, int startSample = 0, int endSample = 0)
        {
            TxthHandler.UpdateTxthFile($"{encodedSong}.txth", startSample, endSample);
        }
    }
}