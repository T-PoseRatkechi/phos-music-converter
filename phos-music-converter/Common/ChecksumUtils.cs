using System;
using System.IO;
using System.Security.Cryptography;

namespace PhosMusicConverter.common
{
    internal class ChecksumUtils
    {
        /// <summary> Calculates and returns the MD5 hash of <paramref name="file"/>.</summary>
        /// <param name="file">Path of file to calculate hash for.</param>
        /// <returns>MD5 hash of <paramref name="file"/></returns>
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
    }
}