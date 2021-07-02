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
            // store txth name for logging
            string txthName = Path.GetFileName(outputFilePath);

            // Get samples per block from first byte of extra params.
            byte samplesPerBlock = props.ExtraParamsBytes[0];

            // Build txth string.
            StringBuilder txthBuilder = new();
            txthBuilder.AppendLine($"num_samples = {AlignToBlock(txthName, totalSamples, samplesPerBlock)}");
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
                txthBuilder.AppendLine($"loop_end_sample = {AlignToBlock(txthName, totalSamples, samplesPerBlock)}");
            }
            else
            {
                int finalStartSample = AlignToBlock(txthName, startSample, samplesPerBlock);
                int finalEndSample = AlignToBlock(txthName, endSample, samplesPerBlock);

                // verify loop points are valid
                if (!IsValidLoop(txthName, totalSamples, finalStartSample, finalEndSample))
                {
                    Output.Log(LogLevel.WARN, $"{txthName}: Loop points were invalid! Defaulting to full song loop!");
                    finalStartSample = 0;
                    finalEndSample = AlignToBlock(txthName, totalSamples, samplesPerBlock);
                }

                // add loop points given
                txthBuilder.AppendLine($"loop_start_sample = {finalStartSample}");
                txthBuilder.AppendLine($"loop_end_sample = {finalEndSample}");
            }

            // write txth to file
            File.WriteAllText(outputFilePath, txthBuilder.ToString());
            Output.Log(LogLevel.INFO, $"{txthName}: Created txth file.");
        }

        /// <summary>
        /// Updates a txth's start loop sample and end loop sample.
        /// </summary>
        /// <param name="txthPath">Path of txth file to update.</param>
        /// <param name="startSample">The new loop start sample.</param>
        /// <param name="endSample">The new loop end sample.</param>
        public static void UpdateTxthFile(string txthPath, int startSample, int endSample)
        {
            // Exit early if original txth is missing (shouldn't happen but oh well).
            if (!File.Exists(txthPath))
            {
                throw new FileNotFoundException("Expected txth file was not found!", txthPath);
            }

            // Store txth name for output.
            string txthName = Path.GetFileName(txthPath);

            // Samples per block. Assumed default is 128.
            byte samplesPerBlock = 128;

            // Path to .extra file that contains a file's extra params.
            string txthExtraPath = $"{txthPath}.extra";

            // Prefer to get samples per block from first byte of extra params, if possible.
            if (File.Exists(txthExtraPath))
            {
                Output.Log(LogLevel.DEBUG, $"{txthName}: Getting samples per block from extra file");
                samplesPerBlock = File.ReadAllBytes(txthExtraPath)[0];
            }
            else
            {
                Output.Log(LogLevel.DEBUG, $"{txthName}: Assuming samples per block is 128");
            }

            string[] originalTxthFile = File.ReadAllLines(txthPath);
            StringBuilder txthBuilder = new();

            int totalSamples = int.Parse(Array.Find<string>(originalTxthFile, s => s.StartsWith("num_samples")).Split(" = ")[1]);
            int finalStartSample = AlignToBlock(txthName, startSample, samplesPerBlock);
            int finalEndSample = AlignToBlock(txthName, endSample, samplesPerBlock);

            // default to full loop if loop points are invalid or both points are 0
            if (!IsValidLoop(txthName, totalSamples, finalStartSample, finalEndSample) || (startSample == 0 && endSample == 0))
            {
                Output.Log(LogLevel.WARN, $"{txthName}: Invalid loop points! Defaulting to full song loop!");
                finalStartSample = 0;
                finalEndSample = AlignToBlock(txthName, totalSamples, samplesPerBlock);
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

        /// <summary>
        /// Sanity checks if the given loop points are valid.
        /// </summary>
        /// <param name="txthName">Name of txth file. Used for logging.</param>
        /// <param name="totalSamples">The total number of samples of the file.</param>
        /// <param name="startSample">The loop start sample.</param>
        /// <param name="endSample">The loop end sample.</param>
        /// <returns><c>true</c> if <paramref name="startSample"/> and <paramref name="endSample"/> are valid samples for looping, <c>false</c> otherwise.</returns>
        private static bool IsValidLoop(string txthName, int totalSamples, int startSample, int endSample)
        {
            if (startSample > totalSamples)
            {
                Output.Log(LogLevel.WARN, $"{txthName}: Loop start sample exceeds total samples: {startSample} > {totalSamples}");
                return false;
            }
            else if (endSample > totalSamples)
            {
                Output.Log(LogLevel.WARN, $"{txthName}: Loop end sample exceeds total samples: {endSample} > {totalSamples}");
                return false;
            }
            else if (startSample > endSample)
            {
                Output.Log(LogLevel.WARN, $"{txthName}: Loop start sample exceeds loop end sample: {startSample} > {endSample}");
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
        private static int AlignToBlock(string txthName, int sample, int perBlock)
        {
            // Check if sample given aligns already.
            if (sample % perBlock != 0)
            {
                // Align sample to block.
                int adjustment = (byte)(sample % perBlock);
                Output.Log(LogLevel.LOG, $"{txthName}: Aligning {sample} to {sample - adjustment} (-{adjustment})");
                return sample - adjustment;
            }
            else
            {
                return sample;
            }
        }
    }
}