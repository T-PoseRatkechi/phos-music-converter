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
            Output.Log(LogLevel.INFO, "Yo dayo!");
            Parser.Default.ParseArguments<CommandOptions.BuildOptions, CommandOptions.BatchOptions>(args)
                .WithParsed<CommandOptions.BuildOptions>(o =>
                {
                    if (o.Verbose)
                    {
                        Output.Verbose = true;
                    }

                    GenerateMusicBuild(o.GameName, o.MusicData, o.EncoderPath, o.OutputDirectory, o.UseLowPerformance);
                })
                .WithParsed<CommandOptions.BatchOptions>(o =>
                {
                    // TODO
                });
        }

        private static void GenerateMusicBuild(string game, string musicDataPath, string encoder, string outputDir, bool useLow)
        {
            try
            {
                IMusicBuilder musicBuilder = null;
                switch (game)
                {
                    case "p4g":
                        musicBuilder = new BuilderP4G(musicDataPath, encoder);
                        break;

                    case "p5":
                        musicBuilder = new BuilderP5(musicDataPath, encoder);
                        break;

                    case "p3f":
                    case "p4":
                        musicBuilder = new BuilderP3F(musicDataPath, encoder);
                        break;

                    default:
                        Output.Log(LogLevel.ERROR, $"Unsupported game option: {game}!");
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
                [Option('f', "folder", Required = true, HelpText = "Directory of files to encode.")]
                public string FolderDirectory { get; set; }

                [Option('e', "encoder", Required = true, HelpText = "Path of encoder to use.")]
                public string EncoderPath { get; set; }

                [Option('l', "low", Required = false, Default = false, HelpText = "Set Phos Music Converter to use less resources. Only use if Phos Music Converter has issues building normally. NOT IMPLEMENTED")]
                public bool UseLowPerformance { get; set; }

                [Option('v', "verbose", Required = false, Default = false, HelpText = "Set output to verbose messages.")]
                public bool Verbose { get; set; }
            }
        }
    }
}