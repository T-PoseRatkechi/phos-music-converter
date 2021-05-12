using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phos_music_converter.builders
{
    class BuilderP3F : BuilderBase
    {
        public BuilderP3F(string path) : base(path, "Persona 3 FES/Persona 4") { }

        public override void GenerateBuild(string outputDir, bool useLow, bool verbose)
        {
            throw new NotImplementedException();
        }
    }
}
