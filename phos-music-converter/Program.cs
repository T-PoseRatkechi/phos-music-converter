// Copyright (c) T-Pose Ratkechi. All rights reserved.
// Licensed under the GNU GPLv3 license. See LICENSE file in the project root for full license information.

namespace PhosMusicConverter
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using CommandLine;
    using PhosMusicConverter.Builders;
    using PhosMusicConverter.Common;
    using PhosMusicConverter.Interfaces;

    /// <summary>
    /// Phos Music Converter Main Class.
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Yo dayo!");
            Parser.Default.ParseArguments<CommandOptions>(args)
                .WithParsed<CommandOptions>(o =>
                {
                    if (o.Verbose)
                    {
                        Output.Verbose = true;
                    }

                    GenerateMusicBuild(o.GameName, o.MusicData, o.OutputDirectory, o.UseLowPerformance);
                });
        }

        private static void GenerateMusicBuild(string game, string musicDataPath, string outputDir, bool useLow)
        {
            try
            {
                IMusicBuilder musicBuilder = null;
                switch (game)
                {
                    case "p4g":
                        musicBuilder = new BuilderP4G(musicDataPath);
                        break;

                    case "p5":
                        musicBuilder = new BuilderP5(musicDataPath);
                        break;

                    case "p3f":
                    case "p4":
                        musicBuilder = new BuilderP3F(musicDataPath);
                        break;

                    default:
                        Console.WriteLine($"Unsupported game option: {game}!");
                        return;
                }

                Stopwatch timer = new();
                timer.Start();
                musicBuilder.GenerateBuild(outputDir, useLow);
                timer.Stop();

                Console.WriteLine($"[INFO] Completed in {timer.ElapsedMilliseconds} ms.");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine($"\nCould not find file: {ex.FileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Failed to generate build!");
            }
        }

        private class CommandOptions
        {
            [Option('g', "game", Required = true, HelpText = "Set what game to generate music build for. Options: p4g, p5, p3f, and p4.")]
            public string GameName { get; set; }

            [Option('i', "input", Required = true, HelpText = "Input music data JSON.")]
            public string MusicData { get; set; }

            [Option('o', "output", Required = true, HelpText = "Directory to generate music build in.")]
            public string OutputDirectory { get; set; }

            [Option('l', "low", Required = false, Default = false, HelpText = "Set Phos Music Converter to use less resources. Only use if Phos Music Converter has issues building normally.")]
            public bool UseLowPerformance { get; set; }

            [Option('v', "verbose", Required = false, Default = false, HelpText = "Set output to verbose messages.")]
            public bool Verbose { get; set; }
        }
    }
}