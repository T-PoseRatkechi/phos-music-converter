// Copyright (c) T-Pose Ratkechi. All rights reserved.
// Licensed under the GNU GPLv3 license. See LICENSE file in the project root for full license information.

namespace PhosLibrary.Builders.Music
{
    using System;
    using PhosLibrary.Common;

    /// <summary>
    /// Music Builder for Persona 3 FES and Persona 4.
    /// </summary>
    internal class BuilderP3F : BuilderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuilderP3F"/> class.
        /// </summary>
        public BuilderP3F()
            : base("Persona 3 FES/Persona 4")
        {
        }

        /// <inheritdoc/>
        public override string EncodedFileExt { get => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public override string[] SupportedFormats { get => throw new NotImplementedException(); }

        /// <inheritdoc/>
        protected override string CachedDirectory => throw new NotImplementedException();

        /// <inheritdoc/>
        protected override string EncoderPath => throw new NotImplementedException();

        /// <inheritdoc/>
        public override void EncodeSong(string songPath, string outPath, int startSample = 0, int endSample = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void GenerateBuild(MusicData musicData, string outputDir, bool useLow)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override void Encode(string inputFile, string outputFile, int startSample = 0, int endSample = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override void ProcessEncodedSong(string encodedSong, int startSample = 0, int endSample = 0)
        {
            throw new NotImplementedException();
        }
    }
}