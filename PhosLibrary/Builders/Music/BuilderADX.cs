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
    /// Music Builder for Persona 3 FES and Persona 4.
    /// </summary>
    internal class BuilderADX : MusicBuilder
    {
        private readonly int encodeRate;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuilderADX"/> class.
        /// </summary>
        /// <param name="gameName">Game name.</param>
        /// <param name="rate">Encode rate.</param>
        public BuilderADX(string gameName, int rate)
            : base(gameName) => this.encodeRate = rate;

        /// <inheritdoc/>
        public override string EncodedFileExt => ".adx";

        /// <inheritdoc/>
        public override string[] SupportedFormats => new string[] { ".wav" };

        /// <inheritdoc/>
        protected override string CachedDirectory => Path.Join(Directory.GetCurrentDirectory(), "cached", "adx");

        /// <inheritdoc/>
        protected override string EncoderPath => Encoders.GetAtomPath();

        /// <inheritdoc/>
        protected override void Encode(string inputFile, string outputFile, int startSample = 0, int endSample = 0)
        {
            ProcessStartInfo encodeInfo = new()
            {
                FileName = this.EncoderPath,
                Arguments = $@"""{inputFile}"" ""{outputFile}"" -codec=ADX -rate={this.encodeRate} {FormatLoopArgs(startSample, endSample)}",
                CreateNoWindow = true,
            };

            Process encoder = Process.Start(encodeInfo);
            encoder.WaitForExit();
        }

        /// <inheritdoc/>
        protected override bool RequiresEncoding(string inputFile, string outFile, int startSample, int endSample)
        {
            // Check if input file has changed since last encoding.
            var inputFileChanged = base.RequiresEncoding(inputFile, outFile, startSample, endSample);

            // Check if loop points have changed.
            var loopChanged = this.HasLoopChanged(inputFile, startSample, endSample);

            return inputFileChanged || loopChanged;
        }

        /// <inheritdoc/>
        protected override void ProcessEncodedSong(string encodedSong, int startSample = 0, int endSample = 0)
        {
        }

        private static string FormatLoopArgs(int start, int end)
        {
            // Loop all (probably?)
            if (start == 0 && end == 0)
            {
                return "-lpa";
            }

            // Set loop points.
            else
            {
                return $"-lps={start} -lpe={end} -nodelterm";
            }
        }

        private bool HasLoopChanged(string inputFile, int startSample, int endSample)
        {
            string loopFile = Path.Join(this.CachedDirectory, $"{Path.GetFileName(inputFile)}.phos");

            // No previous loop points saved.
            if (!File.Exists(loopFile))
            {
                File.WriteAllText(loopFile, $"{startSample}\n{endSample}");
                return true;
            }

            // Compare previously saved loop points to current ones.
            string[] loopLines = File.ReadAllLines(loopFile);

            int prevStart = int.Parse(loopLines[0]);
            int prevEnd = int.Parse(loopLines[1]);

            // Loop points have changed.
            if (prevStart != startSample || prevEnd != endSample)
            {
                Output.Log(LogLevel.DEBUG, $"{Path.GetFileName(inputFile)}: Loop points have changed. Re-encoding...");
                File.WriteAllText(loopFile, $"{startSample}\n{endSample}");
                return true;
            }

            // Loop points are the same.
            else
            {
                Output.Log(LogLevel.DEBUG, $"{Path.GetFileName(inputFile)}: Loop points have not changed.");
                return false;
            }
        }
    }
}