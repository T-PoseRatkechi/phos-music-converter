namespace PhosMusicConverter.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class BuilderP5 : BuilderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuilderP5"/> class. Generates a Music Build for Persona 5.
        /// </summary>
        /// <param name="path">Input music data JSON file.</param>
        public BuilderP5(string path, bool verbose)
            : base(path, "Persona 5", verbose)
        {
        }

        public override void GenerateBuild(string outputDir, bool useLow, bool verbose)
        {
            throw new NotImplementedException();
        }
    }
}