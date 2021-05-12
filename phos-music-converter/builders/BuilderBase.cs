using phos_music_converter.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phos_music_converter.builders
{
    abstract class BuilderBase : IMusicBuilder
    {
        protected MusicData _musicData;

        protected BuilderBase(string musicDataPath, string gameName)
        {
            LoadMusicData(musicDataPath);
            Console.WriteLine($"{gameName} Music Builder");
        }

        private void LoadMusicData(string filePath)
        {
            _musicData = MusicDataParser.ParseMusicData(filePath);
        }

        public abstract void GenerateBuild(string outputDir, bool useLow, bool verbose);
    }
}
