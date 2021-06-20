// Copyright (c) T-Pose Ratkechi. All rights reserved.
// Licensed under the GNU GPLv3 license. See LICENSE file in the project root for full license information.

namespace PhosMusicConverter.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Music Builder for Persona 5.
    /// </summary>
    internal class BuilderP5 : BuilderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuilderP5"/> class. Generates a Music Build for Persona 5.
        /// </summary>
        /// <param name="path">Input music data JSON file.</param>
        /// <param name="encoder">Path to encoder.</param>
        public BuilderP5(string path, string encoder)
            : base("Persona 5", path, encoder)
        {
        }

        /// <inheritdoc/>
        public override string EncodedFileExt => throw new NotImplementedException();

        /// <inheritdoc/>
        public override string[] SupportedFormats => throw new NotImplementedException();

        /// <inheritdoc/>
        protected override string CachedDirectory => throw new NotImplementedException();

        /// <inheritdoc/>
        public override void EncodeSong(string songPath, string outPath, int startSample = 0, int endSample = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void GenerateBuild(string outputDir, bool useLow)
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