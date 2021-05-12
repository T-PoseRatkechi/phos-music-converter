namespace PhosMusicConverter.Interfaces
{
    internal interface IMusicBuilder
    {
        void GenerateBuild(string outputDir, bool useLow, bool verbose);
    }
}