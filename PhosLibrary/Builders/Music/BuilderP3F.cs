// Copyright (c) T-Pose Ratkechi. All rights reserved.
// Licensed under the GNU GPLv3 license. See LICENSE file in the project root for full license information.

namespace PhosLibrary.Builders.Music
{
    using System.Diagnostics;
    using System.IO;
    using PhosLibrary.Common;
    using PhosLibrary.Common.Logging;

    /// <summary>
    /// Music Builder for Persona 3 FES and Persona 4.
    /// </summary>
    internal class BuilderP3F : MusicBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuilderP3F"/> class.
        /// </summary>
        public BuilderP3F()
            : base("Persona 3 FES/Persona 4")
        {
        }

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
                Arguments = $@"""{inputFile}"" ""{outputFile}"" -codec=ADX {FormatLoopArgs(startSample, endSample)}",
                CreateNoWindow = true,
            };

            Process encoder = Process.Start(encodeInfo);
            encoder.WaitForExit();
        }

        /// <inheritdoc/>
        protected override bool RequiresEncoding(string inputFile, string outFile, int startSample, int endSample)
        {
            // Check if input file has changed since last encoding.
            if (base.RequiresEncoding(inputFile, outFile, startSample, endSample))
            {
                return true;
            }

            // Check if loop points have changed.
            return this.HasLoopChanged(inputFile, startSample, endSample);
        }

        /// <inheritdoc/>
        protected override void ProcessEncodedSong(string encodedSong, int startSample = 0, int endSample = 0)
        {
        }

        private bool HasLoopChanged(string inputFile, int startSample, int endSample)
        {
            string loopFile = Path.Join(this.CachedDirectory, $"{Path.GetFileName(inputFile)}.loop");

            if (!File.Exists(loopFile))
            {
                File.WriteAllText(loopFile, $"{startSample}\n{endSample}");
                return true;
            }

            string[] loopLines = File.ReadAllLines(loopFile);

            int prevStart = int.Parse(loopLines[0]);
            int prevEnd = int.Parse(loopLines[1]);

            if (prevStart != startSample || prevEnd != endSample)
            {
                Output.Log(LogLevel.DEBUG, $"{Path.GetFileName(inputFile)}: Loop points have changed. Re-encoding...");
                File.WriteAllText(loopFile, $"{startSample}\n{endSample}");
                return true;
            }
            else
            {
                Output.Log(LogLevel.DEBUG, $"{Path.GetFileName(inputFile)}: Loop points have not changed.");
                return false;
            }
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
    }
}