using System;
using CommandLine;

namespace phos_music_converter
{
    class Program
    {
        public class Options
        {
            [Option('g', "game", Required = true,
                HelpText = "Set what game to generate music build for. Options: p4g, p5, p3f, and p4.")]
            public string GameName { get; set; }
            [Option('i', "input", Required = true,
                HelpText = "File path to input music data JSON.")]
            public string MusicData { get; set; }
            [Option('o', "output", Required = true,
                HelpText = "Directory path to generate music build in.")]
            public string OutputDirectory { get; set; }

            [Option('l', "low", Required = false, Default = false,
                HelpText = "Set Phos Music Converter to use less resources. Only use if Phos Music Converter has issues building normally.")]
            public bool UseLowPerformance { get; set; }
            [Option('v', "verbose", Required = false, Default = false, HelpText = "Set output to verbose messages.")]
            public bool Verbose { get; set; }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Yo dayo!");
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(o =>
                {
                    GenerateMusicBuild(o.GameName, o.MusicData, o.OutputDirectory, o.UseLowPerformance, o.Verbose);
                });
        }

        private static void GenerateMusicBuild(string game, string musicDataPath, string outputDir, bool useLow, bool verbose)
        {

        }
    }
}
