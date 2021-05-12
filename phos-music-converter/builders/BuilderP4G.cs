using phos_music_converter.interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phos_music_converter.builders
{
    class BuilderP4G : BuilderBase
    {
        public BuilderP4G(string path) : base(path, "P4G") { }

        public override void GenerateBuild(string outputDir, bool useLow, bool verbose)
        {
            string currentDir = Directory.GetCurrentDirectory();
            string encoderPath = $@"{currentDir}\..\xacttool_0.1\tools\AdpcmEncode.exe";
            if (!File.Exists(encoderPath))
                throw new FileNotFoundException($"AdpcmEncode.exe could not be found!", Path.GetFullPath(encoderPath));
        }
    }
}
