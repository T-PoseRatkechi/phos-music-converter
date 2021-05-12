// Copyright (c) T-Pose Ratkechi. All rights reserved.
// Licensed under the GNU GPLv3 license. See LICENSE file in the project root for full license information.

namespace PhosMusicConverter.Common
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Useful functions related txth files.
    /// </summary>
    internal class TxthHandler
    {
        /// <summary>
        /// Generates a txth file at <paramref name="outputFilePath"/> from the given <paramref name="props"/> and loop points.
        /// </summary>
        /// <param name="outputFilePath">Output file path of the txth.</param>
        /// <param name="props">The wave's <c>WaveProps</c>.</param>
        /// <param name="totalSamples">Total samples of original file. This information is unobtainable from encoded files.</param>
        /// <param name="startSample">The starting sample of the wave's loop.</param>
        /// <param name="endSample">The ending sample of the wave's loop.</param>
        public static void WriteTxthFile(string outputFilePath, WaveProps props, int totalSamples, int startSample, int endSample)
        {
            // Get samples per block from first byte of extra params.
            byte samplesPerBlock = props.ExtraParamsBytes[0];

            // Build txth string.
            StringBuilder txthBuilder = new();
            txthBuilder.AppendLine($"num_samples = {AlignToBlock(totalSamples, samplesPerBlock)}");
            txthBuilder.AppendLine("codec = MSADPCM");
            txthBuilder.AppendLine($"channels = {props.NumChannels}");
            txthBuilder.AppendLine($"sample_rate = {props.SampleRate}");
            txthBuilder.AppendLine($"interleave = {props.BlockAlign}");

            // Write all extra param bytes.
            File.WriteAllBytes($"{outputFilePath}.extra", props.ExtraParamsBytes);

            // Set txth loop points.
            if (startSample == 0 && endSample == 0)
            {
                // No loop points given, set loop points to full song length.
                txthBuilder.AppendLine("loop_start_sample = 0");
                txthBuilder.AppendLine($"loop_end_sample = {AlignToBlock(totalSamples, samplesPerBlock)}");
            }
            else
            {
                int finalStartSample = AlignToBlock(startSample, samplesPerBlock);
                int finalEndSample = AlignToBlock(endSample, samplesPerBlock);

                // verify loop points are valid
                if (!IsValidLoop(totalSamples, finalStartSample, finalEndSample))
                {
                    Output.Log(LogLevel.WARNING, "Loop points were invalid! Defaulting to full song loop!");
                    finalStartSample = 0;
                    finalEndSample = AlignToBlock(totalSamples, samplesPerBlock);
                }

                // add loop points given
                txthBuilder.AppendLine($"loop_start_sample = {finalStartSample}");
                txthBuilder.AppendLine($"loop_end_sample = {finalEndSample}");
            }

            // write txth to file
            File.WriteAllText(outputFilePath, txthBuilder.ToString());
            Output.Log(LogLevel.INFO, " Created txth file.");
        }

        /// <summary>
        /// Updates a txth's start loop sample and end loop sample.
        /// </summary>
        /// <param name="txthPath">Path of txth file to update.</param>
        /// <param name="startSample">The new loop start sample.</param>
        /// <param name="endSample">The new loop end sample.</param>
        public static void UpdateTxthFile(string txthPath, int startSample, int endSample)
        {
            // exit early if original txth is missing (shouldn't happen but oh well)
            if (!File.Exists(txthPath))
            {
                throw new FileNotFoundException("Expected txth file was not found!", txthPath);
            }

            string[] originalTxthFile = File.ReadAllLines(txthPath);
            StringBuilder txthBuilder = new();

            int totalSamples = int.Parse(Array.Find<string>(originalTxthFile, s => s.StartsWith("num_samples")).Split(" = ")[1]);
            byte samplesPerBlock = File.ReadAllBytes($"{txthPath}.extra")[0];
            int finalStartSample = AlignToBlock(startSample, samplesPerBlock);
            int finalEndSample = AlignToBlock(endSample, samplesPerBlock);

            // default to full loop if loop points are invalid or both points are 0
            if (!IsValidLoop(totalSamples, finalStartSample, finalEndSample) || (startSample == 0 && endSample == 0))
            {
                Output.Log(LogLevel.WARNING, "Invalid loop points! Defaulting to full song loop!");
                finalStartSample = 0;
                finalEndSample = AlignToBlock(totalSamples, samplesPerBlock);
            }

            foreach (string line in originalTxthFile)
            {
                if (line.StartsWith("loop_start_sample"))
                {
                    txthBuilder.AppendLine($"loop_start_sample = {finalStartSample}");
                }
                else if (line.StartsWith("loop_end_sample"))
                {
                    txthBuilder.AppendLine($"loop_end_sample = {finalEndSample}");
                }
                else
                {
                    txthBuilder.AppendLine(line);
                }
            }

            File.WriteAllText(txthPath, txthBuilder.ToString());
        }

        private static bool IsValidLoop(int totalSamples, int startSample, int endSample)
        {
            if (startSample > totalSamples)
            {
                Output.Log(LogLevel.WARNING, $"Loop start sample exceeds total samples: {startSample} > {totalSamples}");
                return false;
            }
            else if (endSample > totalSamples)
            {
                Output.Log(LogLevel.WARNING, $"Loop end sample exceeds total samples: {endSample} > {totalSamples}");
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Returns the nearest block-aligned value of <paramref name="sample"/>, rounding down, according to <paramref name="perBlock"/>.
        /// </summary>
        /// <param name="sample">The original sample to block align.</param>
        /// <param name="perBlock">Number of samples per block.</param>
        /// <returns>Blocked-aligned <paramref name="sample"/>, rounding down.</returns>
        private static int AlignToBlock(int sample, int perBlock)
        {
            // Check if sample given aligns already.
            if (sample % perBlock != 0)
            {
                // Align sample to block.
                int adjustment = (byte)(sample % perBlock);
                Output.Log(LogLevel.LOG, $"Aligning: {sample} to {sample - adjustment} (-{adjustment})");
                return sample - adjustment;
            }
            else
            {
                return sample;
            }
        }
    }
}