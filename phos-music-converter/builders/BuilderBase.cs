using PhosMusicConverter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhosMusicConverter.Builders
{
    internal abstract class BuilderBase : IMusicBuilder
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