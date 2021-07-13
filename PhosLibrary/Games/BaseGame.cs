using PhosLibrary.Common;
using PhosLibrary.Interfaces;

namespace PhosLibrary.Games
{
    internal class BaseGame : IGameMusic
    {
        private abstract
        public void Batch(string inputDir, bool useLow)
        {
            throw new System.NotImplementedException();
        }

        public void Build(MusicData musicData, string outputDir, bool useLow)
        {
            throw new System.NotImplementedException();
        }

        public void Export(MusicData musicData, string outputDir, bool useLow)
        {
            throw new System.NotImplementedException();
        }

        public void Extract(string inputFile, string outputDir)
        {
            throw new System.NotImplementedException();
        }
    }
}