using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phos_music_converter.interfaces
{
    interface IMusicBuilder
    {
        void GenerateBuild(string outputDir, bool useLow, bool verbose);
    }
}
