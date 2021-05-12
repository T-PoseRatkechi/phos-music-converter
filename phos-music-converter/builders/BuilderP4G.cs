using PhosMusicConverter.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhosMusicConverter.Builders
{
    internal class BuilderP4G : BuilderBase
    {
        public BuilderP4G(string path) : base(path, "P4G")
        {
        }

        public override void GenerateBuild(string outputDir, bool useLow, bool verbose)
        {
            string currentDir = Directory.GetCurrentDirectory();

            string encoderPath = $@"{currentDir}\..\xacttool_0.1\tools\AdpcmEncode.exe";
            if (!File.Exists(encoderPath))
                throw new FileNotFoundException($"AdpcmEncode.exe could not be found!", Path.GetFullPath(encoderPath));
        }

        private void EncodeUniqueFiles()
        {
        }

        private void EncodeSong(string songPath, string outPath)
        {
            // check if input file should be re-encoded
            bool waveRequiresEncoding = RequiresEncoding(songPath, outPath);

            // only update txth file if wave doesn't need to be encoded
            if (!waveRequiresEncoding)
            {
                Console.WriteLine("Updating txth file!");
                // store result of txth updated
                bool txthUpdated = txthHandler.UpdateTxthFile($"{outputFilePath}.txth", startSample, endSample);
                return txthUpdated;
            }

            // file path to store temp encoded file (still has header)
            string tempFilePath = $@"{outputFilePath}.temp";

            ProcessStartInfo encodeInfo = new ProcessStartInfo
            {
                FileName = encoderPath,
                Arguments = $@"""{inputFilePath}"" ""{tempFilePath}""",
            };

            // encode file given
            try
            {
                Process process = Process.Start(encodeInfo);
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    Console.WriteLine($"Problem with AdpcmEncode! Exit code: {process.ExitCode}");
                    return false;
                }
            }
            catch (Exception e)
            {
                // problem starting process, exit early
                Console.WriteLine("Problem running AdpcmEncode!");
                Console.WriteLine(e);
                return false;
            }

            // get wave props of input file
            WaveProps inputWaveProps = GetWaveProps(inputFilePath);
            // get num samples from input wave
            int numSamples = GetNumSamples(inputWaveProps);
            if (numSamples <= 0)
            {
                Console.WriteLine("Could not determine number of samples from input wave!");
                return false;
            }

            // get wave props of temp file
            WaveProps outputWaveProps = GetWaveProps(tempFilePath);

            // array to store data chunk bytes
            byte[] outDataChunk = new byte[outputWaveProps.Subchunk2Size];

            try
            {
                // read data chunk into array
                using (FileStream tempfile = File.OpenRead(tempFilePath))
                {
                    tempfile.Seek(outputWaveProps.DataOffset, SeekOrigin.Begin);
                    tempfile.Read(outDataChunk, 0, outDataChunk.Length);
                }
            }
            catch (Exception e)
            {
                // exit early if error reading data chunk
                Console.WriteLine($"Problem reading in data chunk of output!");
                Console.WriteLine(e);
                return false;
            }

            // write txth file
            bool txthSuccess = txthHandler.WriteTxthFile($"{outputFilePath}.txth", outputWaveProps, numSamples, startSample, endSample);
            if (!txthSuccess)
                return false;

            // write raw to file
            try
            {
                // write raw file
                File.WriteAllBytes($"{outputFilePath}", outDataChunk);
                // delete temp file
                File.Delete(tempFilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem writing raw to file!");
                Console.WriteLine(e);
                return false;
            }

            return true;
        }

        // check if file needs to be encoded
        private static bool RequiresEncoding(string infile, string outfile)
        {
            try
            {
                string infileChecksum = GetWaveSum(infile);

                // already encoded file doesn't exist
                if (!File.Exists(outfile))
                {
                    Console.WriteLine("Encoded file missing! Re-encoding required!");
                    return true;
                }

                // checks if saved sum matches infile sum
                if (checksum.GetChecksumString(infile).Equals(infileChecksum))
                {
                    Console.WriteLine("Checksum match! Re-encoding not required...");
                    return false;
                }
                else
                {
                    Console.WriteLine("Checksum mismatch! Re-encoding required!");
                    WriteWaveSum(infile);
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return true;
            }
        }

        // get a wave files saved checksum, create one if missing
        private static string GetWaveSum(string filePath)
        {
            string waveChecksumFile = $"{Path.GetFileName(filePath)}.music";
            string checksumFilePath = $@"{checksumsFolderPath}\{waveChecksumFile}";

            // check if a checksum file for song exists
            if (!File.Exists(checksumFilePath))
            {
                WriteWaveSum(filePath);
                return null;
            }
            else
            {
                try
                {
                    string savedSum = File.ReadAllText(checksumFilePath);
                    return savedSum;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Problem reading wave file checksum!");
                    Console.WriteLine(e);
                    return null;
                }
            }
        }
    }
}