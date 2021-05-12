using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phos_music_converter.builders
{
    class BuilderP5 : BuilderBase
    {
        public BuilderP5(string path) : base(path, "Persona 5") { }

        public override void GenerateBuild(string outputDir, bool useLow, bool verbose)
        {
            throw new NotImplementedException();
        }
    }
}
