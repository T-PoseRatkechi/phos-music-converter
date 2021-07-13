using PhosLibrary.Common;

namespace PhosLibrary.Interfaces
{
    public interface IGameMusic
    {
        public void Build(MusicData musicData, string outputDir, bool useLow);

        public void Export(MusicData musicData, string outputDir, bool useLow);

        public void Batch(string inputDir, bool useLow);

        public void Extract(string inputFile, string outputDir);
    }
}