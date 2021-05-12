using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhosMusicConverter.Builders
{
    /// <summary>
    /// Music Builder for Persona 3 FES and Persona 4.
    /// </summary>
    internal class BuilderP3F : BuilderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuilderP3F"/> class.
        /// </summary>
        /// <param name="path">Input music data JSON file.</param>
        /// <param name="verbose">Verbose setting for errors.</param>
        public BuilderP3F(string path, bool verbose)
            : base(path, "Persona 3 FES/Persona 4", verbose)
        {
        }

        /// <inheritdoc/>
        protected override string CachedDirectory => throw new NotImplementedException();

        /// <inheritdoc/>
        public override void GenerateBuild(string outputDir, bool useLow, bool verbose)
        {
            throw new NotImplementedException();
        }
    }
}