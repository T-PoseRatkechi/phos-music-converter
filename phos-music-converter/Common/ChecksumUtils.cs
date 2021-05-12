// Copyright (c) T-Pose Ratkechi. All rights reserved.
// Licensed under the GNU GPLv3 license. See LICENSE file in the project root for full license information.

namespace PhosMusicConverter.Common
{
    using System;
    using System.IO;
    using System.Security.Cryptography;

    /// <summary>
    /// Collection of utility functions related to checksums.
    /// </summary>
    internal class ChecksumUtils
    {
        private static string checksumDirectory = $@"{Directory.GetCurrentDirectory()}\checksums";

        /// <summary> Calculates and returns the MD5 hash of <paramref name="file"/>.</summary>
        /// <param name="file">Path of file to calculate hash for.</param>
        /// <returns>MD5 hash of <paramref name="file"/>.</returns>
        public static byte[] GetChecksum(string file)
        {
            // get md5 checksum of file
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(file))
                {
                    // return hash
                    return md5.ComputeHash(stream);
                }
            }
        }

        /// <summary>
        /// Returns the locally saved MD5 checksum of <paramref name="file"/>, if it exists. Otherwise, saves <paramref name="file"/>'s
        /// current checksum to file and returns <c>null</c>.
        /// </summary>
        /// <param name="file">File to get saved checksum of.</param>
        /// <returns><c>byte[]</c> containing <paramref name="file"/>'s saved checksum, if it exists. Otherwise, returns <c>null</c>.</returns>
        public static byte[] GetSavedChecksum(string file)
        {
            string savedSumPath = $@"{checksumDirectory}\{Path.GetFileName(file)}.md5";
            if (!File.Exists(savedSumPath))
            {
                byte[] fileSum = GetChecksum(file);
                File.WriteAllBytes(savedSumPath, fileSum);
                return null;
            }
            else
            {
                byte[] savedSum = File.ReadAllBytes(savedSumPath);
                return savedSum;
            }
        }
    }
}