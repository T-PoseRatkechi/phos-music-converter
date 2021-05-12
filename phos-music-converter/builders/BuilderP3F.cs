using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhosMusicConverter.Builders
{
    internal class BuilderP3F : BuilderBase
    {
        public BuilderP3F(string path, bool verbose)
            : base(path, "Persona 3 FES/Persona 4", verbose)
        {
        }

        public override void GenerateBuild(string outputDir, bool useLow, bool verbose)
        {
            throw new NotImplementedException();
        }
    }
}