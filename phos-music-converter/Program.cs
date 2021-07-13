// Copyright (c) T-Pose Ratkechi. All rights reserved.
// Licensed under the GNU GPLv3 license. See LICENSE file in the project root for full license information.

namespace PhosLibrary
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using CommandLine;
    using PhosLibrary.Common.Logging;
    using PhosLibrary.Common.MusicData;
    using PhosLibrary.Games;

    /// <summary>
    /// Phos Music Converter Main Class.
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            Output.Log(LogLevel.INFO, "Yo dayo!");
            Parser.Default.ParseArguments<CommandOptions.BuildOptions, CommandOptions.BatchOptions, CommandOptions.ExtractOptions, CommandOptions.ExportOptions>(args)
                .WithParsed<CommandOptions.BuildOptions>(o =>
                {
                    if (o.Verbose)
                    {
                        Output.Verbose = true;
                        Output.Log(LogLevel.LOG, "Show debug messages enabled");
                    }

                    GenerateMusicBuild(o.GameName, o.MusicData, o.OutputDirectory, o.UseLowPerformance);
                })
                .WithParsed<CommandOptions.BatchOptions>(o =>
                {
                    if (o.Verbose)
                    {
                        Output.Verbose = true;
                        Output.Log(LogLevel.LOG, "Show debug messages enabled");
                    }

                    BatchEncode(o.GameName, o.FolderDirectory, o.UseLowPerformance);
                })
                .WithParsed<CommandOptions.ExtractOptions>(o =>
                {
                    if (o.Verbose)
                    {
                        Output.Verbose = true;
                        Output.Log(LogLevel.LOG, "Show debug messages enabled");
                    }

                    ExtractMusic(o.GameName, o.ExtractFile, o.OutputDirectory);
                })
                .WithParsed<CommandOptions.ExportOptions>(o =>
                 {
                     if (o.Verbose)
                     {
                         Output.Verbose = true;
                         Output.Log(LogLevel.LOG, "Show debug messages enabled");
                     }

                     Export(o.GameName, o.MusicData, o.OutputDirectory, o.UseLowPerformance);
                 });
        }

        private static void ExtractMusic(string game, string inputFile, string outputDir)
        {
            try
            {
                IGameMusic gameMusic = GetGameMusic(game);
                gameMusic.Extract(inputFile, outputDir);
            }
            catch (FileNotFoundException ex)
            {
                Output.Log(LogLevel.ERROR, ex.ToString());
                Output.Log(LogLevel.ERROR, $"Could not find file: {ex.FileName}");
            }
            catch (Exception ex)
            {
                Output.Log(LogLevel.ERROR, ex.ToString());
                Output.Log(LogLevel.ERROR, "Failed to extract music!");
            }
        }

        private static void Export(string game, string musicDataPath, string outputDir, bool useLow)
        {
            try
            {
                IGameMusic gameMusic = GetGameMusic(game);
                if (gameMusic == null)
                {
                    return;
                }

                MusicData musicData = MusicDataParser.Parse(musicDataPath);

                Stopwatch timer = new();
                timer.Start();

                gameMusic.Export(musicData, outputDir, useLow);

                timer.Stop();

                Output.Log(LogLevel.INFO, $"Completed in {timer.ElapsedMilliseconds} ms");
            }
            catch (FileNotFoundException ex)
            {
                Output.Log(LogLevel.ERROR, ex.ToString());
                Output.Log(LogLevel.ERROR, $"Could not find file: {ex.FileName}");
            }
            catch (Exception ex)
            {
                Output.Log(LogLevel.ERROR, ex.ToString());
                Output.Log(LogLevel.ERROR, "Failed to generate build!");
            }
        }

        private static void BatchEncode(string game, string inputDir, bool useLow)
        {
            try
            {
                IGameMusic gameMusic = GetGameMusic(game);
                if (gameMusic == null)
                {
                    return;
                }

                Stopwatch timer = new();
                timer.Start();

                gameMusic.Batch(inputDir, useLow);

                timer.Stop();

                Output.Log(LogLevel.INFO, $"Completed in {timer.ElapsedMilliseconds} ms");
            }
            catch (FileNotFoundException ex)
            {
                Output.Log(LogLevel.ERROR, ex.ToString());
                Output.Log(LogLevel.ERROR, $"Could not find file: {ex.FileName}");
            }
            catch (Exception ex)
            {
                Output.Log(LogLevel.ERROR, ex.ToString());
                Output.Log(LogLevel.ERROR, "Failed to generate build!");
            }
        }

        private static void GenerateMusicBuild(string game, string musicDataPath, string outputDir, bool useLow)
        {
            try
            {
                IGameMusic gameMusic = GetGameMusic(game);
                if (gameMusic == null)
                {
                    return;
                }

                MusicData musicData = MusicDataParser.Parse(musicDataPath);

                Stopwatch timer = new();

                timer.Start();
                gameMusic.Build(musicData, outputDir, useLow);
                timer.Stop();

                Output.Log(LogLevel.INFO, $"Completed in {timer.ElapsedMilliseconds} ms");
            }
            catch (FileNotFoundException ex)
            {
                Output.Log(LogLevel.ERROR, ex.ToString());
                Output.Log(LogLevel.ERROR, $"Could not find file: {ex.FileName}");
            }
            catch (Exception ex)
            {
                Output.Log(LogLevel.ERROR, ex.ToString());
                Output.Log(LogLevel.ERROR, "Failed to generate build!");
            }
        }

        private static IGameMusic GetGameMusic(string gameFlag)
        {
            switch (gameFlag)
            {
                case "p4g":
                    return new MusicP4G();

                case "p5":
                    return new MusicP5();

                case "p3f":
                case "p4":
                    return new MusicP3F();

                default:
                    Output.Log(LogLevel.ERROR, $"Unsupported game option: {gameFlag}!");
                    return null;
            }
        }

        private class CommandOptions
        {
            [Verb("build", HelpText = "Generate a Music Build.")]
            public class BuildOptions
            {
                [Option('g', "game", Required = true, HelpText = "Set what game to generate music build for. Options: p4g, p5, p3f, and p4.")]
                public string GameName { get; set; }

                [Option('i', "input", Required = true, HelpText = "Input music data JSON.")]
                public string MusicData { get; set; }

                [Option('o', "output", Required = true, HelpText = "Directory to generate music build in.")]
                public string OutputDirectory { get; set; }

                [Option('l', "low", Required = false, Default = false, HelpText = "Set Phos Music Converter to use less resources. Only use if Phos Music Converter has issues building normally. NOT IMPLEMENTED")]
                public bool UseLowPerformance { get; set; }

                [Option('v', "verbose", Required = false, Default = false, HelpText = "Set output to verbose messages.")]
                public bool Verbose { get; set; }
            }

            [Verb("batch", HelpText = "Batch encode a folder of files.")]
            public class BatchOptions
            {
                [Option('g', "game", Required = true, HelpText = "Set what game to generate music build for. Options: p4g, p5, p3f, and p4.")]
                public string GameName { get; set; }

                [Option('f', "folder", Required = true, HelpText = "Directory of files to encode.")]
                public string FolderDirectory { get; set; }

                [Option('l', "low", Required = false, Default = false, HelpText = "Set Phos Music Converter to use less resources. Only use if Phos Music Converter has issues building normally. NOT IMPLEMENTED")]
                public bool UseLowPerformance { get; set; }

                [Option('v', "verbose", Required = false, Default = false, HelpText = "Set output to verbose messages.")]
                public bool Verbose { get; set; }
            }

            [Verb("export", HelpText = "Export a music build's encoded song files.")]
            public class ExportOptions
            {
                [Option('g', "game", Required = true, HelpText = "Set what game to export music build for. Options: p4g, p5, p3f, and p4.")]
                public string GameName { get; set; }

                [Option('i', "input", Required = true, HelpText = "Input music data JSON.")]
                public string MusicData { get; set; }

                [Option('o', "output", Required = true, HelpText = "Directory to export music build in.")]
                public string OutputDirectory { get; set; }

                [Option('l', "low", Required = false, Default = false, HelpText = "Set Phos Music Converter to use less resources. Only use if Phos Music Converter has issues building normally. NOT IMPLEMENTED")]
                public bool UseLowPerformance { get; set; }

                [Option('v', "verbose", Required = false, Default = false, HelpText = "Set output to verbose messages.")]
                public bool Verbose { get; set; }
            }

            [Verb("extract", HelpText = "Extract songs from supported file types.")]
            public class ExtractOptions
            {
                [Option('g', "game", Required = true, HelpText = "Set what game to extract music for. Options: p4g, p5, p3f, and p4.")]
                public string GameName { get; set; }

                [Option('i', "input", Required = true, HelpText = "Path of file to extract from.")]
                public string ExtractFile { get; set; }

                [Option('o', "output", Required = true, HelpText = "Directory path to output extracted files.")]
                public string OutputDirectory { get; set; }

                [Option('v', "verbose", Required = false, Default = false, HelpText = "Set output to verbose messages.")]
                public bool Verbose { get; set; }
            }
        }
    }
}