﻿// Copyright (c) T-Pose Ratkechi. All rights reserved.
// Licensed under the GNU GPLv3 license. See LICENSE file in the project root for full license information.

namespace PhosMusicConverter
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using CommandLine;
    using PhosMusicConverter.Builders;
    using PhosMusicConverter.Common;

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

                    GenerateMusicBuild(o.GameName, o.MusicData, o.EncoderPath, o.OutputDirectory, o.UseLowPerformance);
                })
                .WithParsed<CommandOptions.BatchOptions>(o =>
                {
                    if (o.Verbose)
                    {
                        Output.Verbose = true;
                        Output.Log(LogLevel.LOG, "Show debug messages enabled");
                    }

                    BatchEncode(o.GameName, o.FolderDirectory, o.EncoderPath, o.UseLowPerformance);
                })
                .WithParsed<CommandOptions.ExtractOptions>(o =>
                {
                    if (o.Verbose)
                    {
                        Output.Verbose = true;
                        Output.Log(LogLevel.LOG, "Show debug messages enabled");
                    }

                    ExtractMusic(o.ExtractFile, o.OutputDirectory);
                })
                .WithParsed<CommandOptions.ExportOptions>(o =>
                 {
                     if (o.Verbose)
                     {
                         Output.Verbose = true;
                         Output.Log(LogLevel.LOG, "Show debug messages enabled");
                     }

                     Export(o.GameName, o.MusicData, o.EncoderPath, o.OutputDirectory, o.UseLowPerformance);
                 });
        }

        private static void ExtractMusic(string inputFile, string outputDir)
        {
            try
            {
                string inputFileType = Path.GetExtension(inputFile).ToLower();
                switch (inputFileType)
                {
                    case ".xwb":
                        Output.Log(LogLevel.INFO, "Extracting music from XWB");
                        Output.Log(LogLevel.INFO, "Uses code from unxwb by Luigi Auriemma.\nLicensed under the GNU GPLv3 license. See unxwb.LICENSE file in the project root for full license information.");
                        XwbUtils.ExtractSongs(inputFile, outputDir);
                        Output.Log(LogLevel.INFO, "Finished extracting music from XWB");
                        break;

                    default:
                        Output.Log(LogLevel.ERROR, $"Unrecognized file type: {inputFileType}");
                        break;
                }
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

        private static void Export(string game, string musicDataPath, string encoder, string outputDir, bool useLow)
        {
            try
            {
                BuilderBase musicBuilder = GetGameBuilder(game, encoder, musicDataPath);
                if (musicBuilder == null)
                {
                    return;
                }

                Stopwatch timer = new();
                timer.Start();

                ExportBuilder.Export(musicBuilder, outputDir, useLow);

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

        private static void BatchEncode(string game, string inputDir, string encoder, bool useLow)
        {
            try
            {
                BuilderBase musicBuilder = GetGameBuilder(game, encoder);
                if (musicBuilder == null)
                {
                    return;
                }

                Stopwatch timer = new();
                timer.Start();

                BatchBuilder.Batch(musicBuilder, inputDir, useLow);

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

        private static void GenerateMusicBuild(string game, string musicDataPath, string encoder, string outputDir, bool useLow)
        {
            try
            {
                BuilderBase musicBuilder = GetGameBuilder(game, encoder, musicDataPath);
                if (musicBuilder == null)
                {
                    return;
                }

                Stopwatch timer = new();
                timer.Start();
                musicBuilder.GenerateBuild(outputDir, useLow);
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

        private static BuilderBase GetGameBuilder(string game, string encoder, string musicDataPath = null)
        {
            switch (game)
            {
                case "p4g":
                    return new BuilderP4G(musicDataPath, encoder);

                case "p5":
                    return new BuilderP5(musicDataPath, encoder);

                case "p3f":
                case "p4":
                    return new BuilderP3F(musicDataPath, encoder);

                default:
                    Output.Log(LogLevel.ERROR, $"Unsupported game option: {game}!");
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

                [Option('e', "encoder", Required = true, HelpText = "Path of encoder to use.")]
                public string EncoderPath { get; set; }

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

                [Option('e', "encoder", Required = true, HelpText = "Path of encoder to use.")]
                public string EncoderPath { get; set; }

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

                [Option('e', "encoder", Required = true, HelpText = "Path of encoder to use.")]
                public string EncoderPath { get; set; }

                [Option('l', "low", Required = false, Default = false, HelpText = "Set Phos Music Converter to use less resources. Only use if Phos Music Converter has issues building normally. NOT IMPLEMENTED")]
                public bool UseLowPerformance { get; set; }

                [Option('v', "verbose", Required = false, Default = false, HelpText = "Set output to verbose messages.")]
                public bool Verbose { get; set; }
            }

            [Verb("extract", HelpText = "Extract songs from supported file types.")]
            public class ExtractOptions
            {
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